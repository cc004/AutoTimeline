using ExcelDataReader;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CalcImport
{
    class Program
	{
		private static Dictionary<int, string[]> characters = JsonConvert.DeserializeObject<Dictionary<int, string[]>>(File.ReadAllText("characters.json"));
		[STAThread]
		
		static void Main(string[] args)
        {
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "excel|*.xlsx";
			if (dialog.ShowDialog() != DialogResult.OK) return;

			var data = ExcelReaderFactory.CreateOpenXmlReader(File.OpenRead(dialog.FileName), null).AsDataSet()
				.Tables["轴模板"].Rows;

			var j = 9;
			List<Tuple<int, int, string>> timeline = new List<Tuple<int, int, string>>();

			while (data[j][10] is double @double)
            {
				timeline.Add(new Tuple<int, int, string>((int)@double, (int)(double)data[j][11], data[j][1].ToString()));
				++j;
            }

			var src = new StringBuilder();

			foreach (var unit in new HashSet<int>(timeline.Select(t => t.Item2).Where(t => t < 200000)))
			{
				src.AppendLine($"print(\"calibrate for {characters[unit / 100].First()}\");");
				src.AppendLine($"autopcr.calibrate(\"{characters[unit / 100].First()}\");");
			}

			src.AppendLine("autopcr.setOffset(2, 0); --offset calibration");

			var offset = 0;
			foreach (var ub in timeline)
			{
				if (ub.Item2 < 200000)
					src.AppendLine($"autopcr.waitFrame({ub.Item1}); autopcr.press(\"{characters[ub.Item2 / 100].First()}\"); --{ub.Item3}");
				else
					src.AppendLine("print(\"boss ub\");");
			}

			File.WriteAllText("timeline.lua", src.ToString());

			Console.Write("已经保存到timeline.lua");
			Console.ReadLine();
		}
    }
}
