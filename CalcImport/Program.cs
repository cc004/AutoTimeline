using Newtonsoft.Json;
using PCRCaculator;
using PCRCaculator.Guild;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CalcImport
{
    class Program
	{
		private static Dictionary<int, int> ubtime = new Dictionary<int, int>();
		private static Dictionary<int, string[]> characters = JsonConvert.DeserializeObject<Dictionary<int, string[]>>(File.ReadAllText("characters.json"));

		private static int CacheGetUbTime(int unit)
		{
			if (ubtime.TryGetValue(unit, out var val)) return val;

			var textAsset = File.ReadAllText($"unitPrefabDatas/UNIT_{unit}.json");
			var time = JsonConvert.DeserializeObject<UnitPrefabData>(textAsset).UnitActionControllerData.UnionBurstList.First().BlackOutTime;

			float dtime = 1 / 60f, counter = 0;
			var frame = 1;
			while ((counter += dtime) <= time) ++frame;

			ubtime[unit] = frame;
			return frame;
		}

		private static void SaveTimeline(GuildTimelineData timelineData, string path)
		{
			var src = new StringBuilder();

			foreach (var unit in timelineData.playerGroupData.playerData.playrCharacters)
			{
				src.AppendLine($"print(\"calibrate for {characters[unit.unitId / 100].First()}\");");
				src.AppendLine($"autopcr.calibrate({unit.unitId});");
			}

			src.AppendLine("autopcr.setOffset(2, 0); --offset calibration");
			
			var offset = 0;
			foreach (var ub in timelineData.playerGroupData.UBExecTimeData
				.SelectMany((list, i) => list
					.Select(data => new Tuple<int, int>(timelineData.playerGroupData.playerData.playrCharacters[i].unitId, (int)Math.Round(60 * (90 - data)))))
				.OrderBy(tuple => tuple.Item2))
			{
				src.AppendLine($"autopcr.waitFrame({offset + ub.Item2}); autopcr.press({ub.Item1});");
				offset += CacheGetUbTime(ub.Item1);
			}
			
			File.WriteAllText(path, src.ToString());
		}

		private static byte[] Keys = new byte[]
	   {
		32,
		32,
		120,
		37,
		206,
		55,
		102,
		byte.MaxValue
	   };
		public static string DecryptDES(string decryptString, string decryptKey = "PCRGuild")
		{
			string result;
			try
			{
				byte[] bytes = Encoding.UTF8.GetBytes(decryptKey);
				byte[] keys = Keys;
				byte[] array = Convert.FromBase64String(decryptString);
				DESCryptoServiceProvider descryptoServiceProvider = new DESCryptoServiceProvider();
				MemoryStream memoryStream = new MemoryStream();
				CryptoStream cryptoStream = new CryptoStream(memoryStream, descryptoServiceProvider.CreateDecryptor(bytes, keys), CryptoStreamMode.Write);
				cryptoStream.Write(array, 0, array.Length);
				cryptoStream.FlushFinalBlock();
				result = Encoding.UTF8.GetString(memoryStream.ToArray());
			}
			catch (Exception)
			{
				result = decryptString;
			}
			return result;
		}

		static void Main(string[] args)
        {
			if (!File.Exists("code.txt"))
			{
				Console.Write("请把savePage的代码命名为code.txt放在目录下");
				Console.ReadLine();
				return;
			}
			var data = JsonConvert.DeserializeObject<GuildTimelineData>(DecryptDES(File.ReadAllText("code.txt")));

			SaveTimeline(data, "timeline.lua");
			Console.Write("已经保存到timeline.lua");
			Console.ReadLine();
		}
    }
}
