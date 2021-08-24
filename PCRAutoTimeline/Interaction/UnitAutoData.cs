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
		private class UnitAuto
		{
			public long unit_id;
			public string unit_name;
			public int atk_type;
			public float atk_cast_time;
			public int ub_type;
			public float atk_prefab_data;
			public int self_buff_id;
			public float self_buff_time;
		}


		private static List<UnitAuto> unit_auto_data;

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


		private class PartsInfo //开成类，等多部位功能完善以后，可以增加另一部分的数据
		{
			public string part_name;
			public long part_id;
			public long father_id;
			public bool Contains(long target_id)
			{
				return target_id == part_id ? true : false;
			}
		}
		private class BossPartsInfo
        {
			public string BossName;
			public long EnemyId;
			public long ChildId1;
			public long ChildId2;
			public long ChildId3;
			public long ChildId4;
			public long ChildId5;
			public List<PartsInfo> ChildArray = new List<PartsInfo>();

			public bool ContainChild(long id)
			{
					return ChildArray.FirstOrDefault(u => u.Contains(id))?.part_id==null ? false : true;
			}
			public void ArrayInit() //将来多部位功能更新以后，这个初始化函数估计要改
			{
				if (ChildId1 != 0)
				{ ChildArray.Add(new PartsInfo {part_name="一号部位", part_id=ChildId1,father_id=EnemyId }); }
				if (ChildId2 != 0)
				{ ChildArray.Add(new PartsInfo { part_name = "二号部位", part_id = ChildId2, father_id = EnemyId }); }
				if (ChildId3 != 0)
				{ ChildArray.Add(new PartsInfo { part_name = "三号部位", part_id = ChildId3, father_id = EnemyId }); }
				if (ChildId4 != 0)
				{ ChildArray.Add(new PartsInfo { part_name = "四号部位", part_id = ChildId4, father_id = EnemyId }); }
				if (ChildId5 != 0)
				{ ChildArray.Add(new PartsInfo { part_name = "五号部位", part_id = ChildId5, father_id = EnemyId }); }
			}
		}

		private static List<BossPartsInfo> bossPartsInfos;

		public static void Init() 
		{
			string str = File.ReadAllText("Data/UnitAutoData.json");
			unit_auto_data = JsonConvert.DeserializeObject<List<UnitAuto>>(str);

			string str2 = File.ReadAllText("Data/unit_auto_prefab.json");
			units = JsonConvert.DeserializeObject<List<UnitData>>(str2);

			string str3 = File.ReadAllText("Data/clan_boss_info.json");
			bossInfos = JsonConvert.DeserializeObject<List<BossInfo>>(str3);

			string str4 = File.ReadAllText("Data/boss_parts_info.json");
			bossPartsInfos = JsonConvert.DeserializeObject<List<BossPartsInfo>>(str4);

			foreach (BossPartsInfo boss_part in bossPartsInfos)
			{
				boss_part.ArrayInit();
			}

		}

		public static int getUbTypeFromId(long unit_id)
        {
			return unit_auto_data.FirstOrDefault(u => u.unit_id == unit_id)?.ub_type ?? -1;
		}
		public static int getAtkType(long unit_id)
		{
			return unit_auto_data.FirstOrDefault(u => u.unit_id == unit_id)?.atk_type ?? -1;

		}

		public static int getSelfBuffId(long unit_id)
		{
			return unit_auto_data.FirstOrDefault(u => u.unit_id == unit_id)?.self_buff_id ?? -1;
		}

		public static float getSelfBuffTime(long unit_id)
		{
			return unit_auto_data.FirstOrDefault(u => u.unit_id == unit_id)?.self_buff_time ?? -1;
		}

		public static string getUnitName(long unit_id) 
		{
			return unit_auto_data.FirstOrDefault(u => u.unit_id == unit_id)?.unit_name ?? "未知角色";

		}

		public static int getAtkPrefabFrame(long unit_id)//如果找不到数据返回-1（可能是有弹道角色）
		{
			var time= unit_auto_data.FirstOrDefault(u => u.unit_id == unit_id)? .atk_prefab_data ?? -1;
			return time == -1 ? -1 : ((int)Math.Ceiling(60 * time));
		}

		public static int getSkillExFrame(long unit_id, long skillid)
		{

			var actions=units.Where(u => u.prefab == unit_id).Select(i => (i.actions,i.exectime))?.Where(d => d.actions.ContainsKey(skillid)).Select(d => (d.exectime.Where(v=>d.actions[skillid].Contains(v.Key)).Select(v=>v.Value))).FirstOrDefault();
			return actions != null ?(int) actions.Max() : 0;

		}

		public static string getBossName(long boss_id)
		{
			BossInfo? boss_data = bossInfos.FirstOrDefault(u => u.EnemyId == boss_id);
			return boss_data==null?"未找到该Boss": $"工会战ID为{boss_data.ClanBattleId} 第{boss_data.Phase}阶段的BOSS {boss_data.BossName}" ;
		}

		public static int getBossPhase(long boss_id)
		{
			return bossInfos.FirstOrDefault(u => u.EnemyId == boss_id)?.Phase??-1;
		}

		public static int getBossClanId(long boss_id)
		{
			return bossInfos.FirstOrDefault(u => u.EnemyId == boss_id)?.ClanBattleId ?? -1;
		}


		public static string getBossPartsName(long boss_part_id)
		{
			return bossPartsInfos.FirstOrDefault(u => u.ContainChild(boss_part_id))?.ChildArray.FirstOrDefault(u => u.Contains(boss_part_id)).part_name ??"未知部位";
		}

		public static (string, long)[] getBossChildId(long boss_id)
		{
				return bossPartsInfos.FirstOrDefault(u=>u.EnemyId==boss_id)?.ChildArray.Select(s => (s.part_name,s.part_id)).ToArray();
		}

		public static long getFatherId(long boss_part_id)//将来PartsInfo实例化了以后这个方法应该要改写
		{
			return bossPartsInfos.FirstOrDefault(x =>x.ContainChild(boss_part_id))?.EnemyId ?? -1;
		}

	}
}
