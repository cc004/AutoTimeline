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

		private static void SaveTimeline(Timeline timeline, string path)
		{
			var timelineData = timeline.timeline;
			var src = new StringBuilder();

			foreach (var unit in timelineData.playerGroupData.playerData.playrCharacters)
			{
				src.AppendLine($"print(\"calibrate for {characters[unit.unitId / 100].First()}\");");
				src.AppendLine($"autopcr.calibrate({unit.unitId});");
			}

			src.AppendLine("autopcr.setOffset(2, 0); --offset calibration");

			var offset = 0;
			foreach (var ub in timeline.state
				.SelectMany(pair => pair.Value.Where(data => data.changStateTo == ActionState.SKILL_1)
					.Select(data => new Tuple<int, int>(pair.Key, data.currentFrameCount)))
				.OrderBy(tuple => tuple.Item2))
			{
				if (timelineData.playerGroupData.playerData.playrCharacters.Any(c => c.unitId == ub.Item1))
					src.AppendLine($"autopcr.waitFrame({offset + ub.Item2}); autopcr.press({ub.Item1});");
				offset += CacheGetUbTime(ub.Item1);
			}

			File.WriteAllText(path, src.ToString());
		}

		static void Main(string[] args)
        {
			if (!File.Exists("timeline.json"))
			{
				Console.Write("请把摸轴器生成的timeline.json放在目录下");
				Console.ReadLine();
				return;
			}
			var data = JsonConvert.DeserializeObject<Timeline>((File.ReadAllText("timeline.json")));

			SaveTimeline(data, "timeline.lua");
			Console.Write("已经保存到timeline.lua");
			Console.ReadLine();
		}
    }
}
