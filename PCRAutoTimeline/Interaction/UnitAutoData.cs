using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;



namespace PCRAutoTimeline.Interaction
{
    class UnitAutoData
    {
		private class Unit_auto
		{
			public string unit_id { get; set; }
			public string unit_name { get; set; }
			public string atk_type { get; set; }
			public string atk_cast_time { get; set; }
			public string ub_type { get; set; }
			public string atk_prefab_data { get; set; }
		}

		private class RootObject
		{
			public List<Unit_auto> data_list { get; set; }
		}

		private static RootObject unit_auto_data;

		private class UnitData
		{
			public int prefab;
			public readonly Dictionary<long, long> exectime = new Dictionary<long, long>();
			public readonly Dictionary<long, long[]> actions = new Dictionary<long, long[]>();
		}

		private static List<UnitData> units;

		private class BossInfo
		{
			public int Phase;
			public int ClanBattleId;
			public string BossName;
			public long EnemyId;
		}

		private static List<BossInfo> bossInfos;

		private class BossPartsInfo
        {
			public string BossName;
			public long EnemyId;
			public long ChildId1;
			public long ChildId2;
			public long ChildId3;
			public long ChildId4;
			public long ChildId5;
		}

		private static List<BossPartsInfo> bossPartsInfos;

		public static void Init() 
		{
			FileStream fs = new FileStream("Data/UnitAutoData.json", FileMode.Open);
			StreamReader fileStream = new StreamReader(fs);
			string str = "";
			string line;
			while ((line = fileStream.ReadLine()) != null)
			{
				str += line;
			}
			unit_auto_data = JsonConvert.DeserializeObject<RootObject>(str);

			FileStream fs2 = new FileStream("Data/unit_auto_prefab.json", FileMode.Open);
			StreamReader fileStream2 = new StreamReader(fs2);
			string str2 = "";
			string line2;
			while ((line2 = fileStream2.ReadLine()) != null)
			{
				str2 += line2;
			}
			units = JsonConvert.DeserializeObject<List<UnitData>>(str2);


			FileStream fs3 = new FileStream("Data/clan_boss_info.json", FileMode.Open);
			StreamReader fileStream3 = new StreamReader(fs3);
			string str3 = "";
			string line3;
			while ((line3 = fileStream3.ReadLine()) != null)
			{
				str3 += line3;
			}
			bossInfos = JsonConvert.DeserializeObject<List<BossInfo>>(str3);

			FileStream fs4 = new FileStream("Data/boss_parts_info.json", FileMode.Open);
			StreamReader fileStream4 = new StreamReader(fs4);
			string str4 = "";
			string line4;
			while ((line4 = fileStream4.ReadLine()) != null)
			{
				str4 += line4;
			}
			bossPartsInfos = JsonConvert.DeserializeObject<List<BossPartsInfo>>(str4);

		}



		public static int GetUbTypeFromId(long unit_id)
        {
			foreach (var unit_data in unit_auto_data.data_list)
            {
				if (long.Parse(unit_data.unit_id) == unit_id)
                {

					var out_data = Int32.Parse(unit_data.ub_type);
					return out_data;

				}
            }
			return -1;
        }
		public static int GetAtkType(long unit_id)
		{
			foreach (var unit_data in unit_auto_data.data_list)
			{
				if (long.Parse(unit_data.unit_id) == unit_id)
				{

					var out_data = Int32.Parse(unit_data.atk_type);
					return out_data;

				}
			}
			return -1;
		}


		public static string GetUnitName(int unit_id) 
		{
			foreach (var unit_data in unit_auto_data.data_list)
			{
				if (long.Parse(unit_data.unit_id) == unit_id)
				{

					var out_data = unit_data.unit_name;
					return out_data;

				}
			}
			return "未知角色";

		}

		public static int GetAtkPrefabFrame(long unit_id)//如果找不到数据返回-1（可能是有弹道角色）
		{
			foreach (var unit_data in unit_auto_data.data_list)
			{
				if (long.Parse(unit_data.unit_id) == unit_id)
				{

					var out_data = float.Parse(unit_data.atk_prefab_data);
					return (int)Math.Ceiling(60*out_data);

				}
			}
			return -1;
		}

		public static int getSkillExFrame(long unit_id, long skillid)
		{
			if (skillid == 0)
			{
				return 0;
			}
			else
			{
				foreach (UnitData unit in units)
				{
					if (unit.prefab == unit_id)
					{
						var skill_action_frame = new List<long>();
						try
						{
							unit.actions.TryGetValue(skillid, out var skill_action_list);
							foreach (long action_id in skill_action_list)
							{
								unit.exectime.TryGetValue(action_id, out var temp_frame);
								skill_action_frame.Add(temp_frame);


							}
							var max_frame = skill_action_frame.Max();

							return (int)max_frame;
						}
						catch { return 0; }
					}
				}
				return 0;
			}
		}

		public static string GetBossName(long boss_id)
		{
			foreach (var boss_data in bossInfos)
			{
				if (boss_data.EnemyId == boss_id)
				{
					return "工会战ID为" + boss_data.ClanBattleId +" 第"+boss_data.Phase+"阶段的BOSS "+ boss_data.BossName;
				}
			}
			return "未找到该Boss";
		}

		public static int GetBossPhase(long boss_id)
		{
			foreach (var boss_data in bossInfos)
			{
				if (boss_data.EnemyId == boss_id)
				{
					return boss_data.Phase;
				}
			}
			return -1;
		}

		public static int GetBossClanId(long boss_id)
		{
			foreach (var boss_data in bossInfos)
			{
				if (boss_data.EnemyId == boss_id)
				{
					return boss_data.ClanBattleId;
				}
			}
			return -1;
		}


		public static string GetBossPartsName(long boss_part_id)
		{
			foreach (var boss_parts_data in bossPartsInfos)
			{
				if (boss_parts_data.ChildId1 == boss_part_id)
				{
					return boss_parts_data.BossName+" 的1号部位";
				}
				else if (boss_parts_data.ChildId2 == boss_part_id)
				{
					return boss_parts_data.BossName+" 的2号部位";
				}
				else if (boss_parts_data.ChildId3 == boss_part_id)
				{
					return boss_parts_data.BossName + " 的3号部位";
				}
				else if (boss_parts_data.ChildId4 == boss_part_id)
				{
					return boss_parts_data.BossName + " 的4号部位";
				}
				else if (boss_parts_data.ChildId5 == boss_part_id)
				{
					return boss_parts_data.BossName + " 的5号部位";
				}
			}
			return "未知部位";
		}

		public static List<(string, long)> GetBossChildId(long boss_id)
		{
			var ret_list = new List<(string, long)>();
			foreach (var boss_parts_data in bossPartsInfos)
			{
				if (boss_parts_data.EnemyId == boss_id) 
				{
					if (boss_parts_data.ChildId1 != 0) { ret_list.Add(("1号部位", boss_parts_data.ChildId1)); }
					if (boss_parts_data.ChildId2 != 0) { ret_list.Add(("2号部位", boss_parts_data.ChildId2)); }
					if (boss_parts_data.ChildId3 != 0) { ret_list.Add(("3号部位", boss_parts_data.ChildId3)); }
					if (boss_parts_data.ChildId4 != 0) { ret_list.Add(("4号部位", boss_parts_data.ChildId4)); }
					if (boss_parts_data.ChildId5 != 0) { ret_list.Add(("5号部位", boss_parts_data.ChildId5)); }
				}
			}
			return ret_list;
		}

		public static long GetFatherId(long boss_part_id)
		{
			foreach (var boss_parts_data in bossPartsInfos)
			{
				if (boss_parts_data.ChildId1 == boss_part_id || boss_parts_data.ChildId2 == boss_part_id || boss_parts_data.ChildId3 == boss_part_id ||
					boss_parts_data.ChildId4 == boss_part_id || boss_parts_data.ChildId5 == boss_part_id)
				{
					return boss_parts_data.EnemyId;
				}
			}
			return -1;

		}

	}
}
