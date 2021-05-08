using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace PCRCaculator
{
	// Token: 0x02000148 RID: 328
	[Serializable]
	public class Skill
	{
		// Token: 0x04000909 RID: 2313
		public bool IsPrincessForm;

		// Token: 0x0400090E RID: 2318
		public bool ForcePlayNoTarget;

		// Token: 0x0400090F RID: 2319
		public int ParameterTarget;

		// Token: 0x04000910 RID: 2320
		private float castTime;

		// Token: 0x04000911 RID: 2321
		public int SkillId;

		// Token: 0x04000912 RID: 2322
		public float skillAreaWidth;

		// Token: 0x04000914 RID: 2324
		public bool Cancel;

		// Token: 0x04000915 RID: 2325
		private int skillNum;

		// Token: 0x04000916 RID: 2326
		private List<int> hasParentIndexes;

		// Token: 0x04000917 RID: 2327
		public float BlackOutTime;

		// Token: 0x04000918 RID: 2328
		public bool BlackoutEndtWithMotion;

		// Token: 0x04000919 RID: 2329
		public bool ForceComboDamage;

		// Token: 0x0400091A RID: 2330
		public float CutInMovieFadeStartTime;

		// Token: 0x0400091B RID: 2331
		public float CutInMovieFadeDurationTime;

		// Token: 0x0400091C RID: 2332
		public float CutInSkipTime;

		// Token: 0x0400091D RID: 2333
		public int Level;

		// Token: 0x0400091E RID: 2334
		public string SkillName = "未命名";
	}
}

namespace PCRCaculator
{
	// Token: 0x020002A4 RID: 676
	public class UnitActionControllerData
	{
		// Token: 0x040020E5 RID: 8421
		public bool UseDefaultDelay;

		// Token: 0x040020E6 RID: 8422
		public Skill Attack;

		// Token: 0x040020E7 RID: 8423
		public List<Skill> UnionBurstList;

		// Token: 0x040020E8 RID: 8424
		public List<Skill> MainSkillList;

		// Token: 0x040020E9 RID: 8425
		public List<Skill> SpecialSkillList;

		// Token: 0x040020EA RID: 8426
		public List<Skill> UnionBurstEvolutionList;

		// Token: 0x040020EB RID: 8427
		public List<Skill> MainSkillEvolutionList;

		// Token: 0x040020EC RID: 8428
		public Skill Annihilation;
	}
}

namespace PCRCaculator
{
	// Token: 0x02000104 RID: 260
	[Serializable]
	public class UnitPrefabData
	{
		// Token: 0x040006EF RID: 1775
		public UnitActionControllerData UnitActionControllerData;

	}
}
namespace PCRCaculator.Guild
{
	// Token: 0x0200011F RID: 287
	public class GuildRandomSpecialData
	{
		// Token: 0x04000862 RID: 2146
		public bool fixTimeExec;

		// Token: 0x04000863 RID: 2147
		public int startFream;

		// Token: 0x04000864 RID: 2148
		public int endFream;

		// Token: 0x04000865 RID: 2149
		public bool fixCountExec;

		// Token: 0x04000866 RID: 2150
		public int countEcexNum;

		// Token: 0x04000867 RID: 2151
		public GuildRandomSpecialData.UnitType sourceNum;

		// Token: 0x04000868 RID: 2152
		public GuildRandomSpecialData.skillNameType sourceSkillNum;

		// Token: 0x04000869 RID: 2153
		public GuildRandomSpecialData.skillType sourceSkillType;

		// Token: 0x0400086A RID: 2154
		public GuildRandomSpecialData.UnitType targetNum;

		// Token: 0x0400086B RID: 2155
		public GuildRandomSpecialData.ResultType resuleType;

		// Token: 0x02000385 RID: 901
		public enum UnitType
		{
			// Token: 0x040024E1 RID: 9441
			[Description("BOSS")]
			BOSS,
			// Token: 0x040024E2 RID: 9442
			[Description("己方一号位")]
			PLAYER1,
			// Token: 0x040024E3 RID: 9443
			[Description("己方二号位")]
			PLAYER2,
			// Token: 0x040024E4 RID: 9444
			[Description("己方三号位")]
			PLAYER3,
			// Token: 0x040024E5 RID: 9445
			[Description("己方四号位")]
			PLAYER4,
			// Token: 0x040024E6 RID: 9446
			[Description("己方五号位")]
			PLAYER5,
			// Token: 0x040024E7 RID: 9447
			[Description("己方全体")]
			ALLPLAYER
		}

		// Token: 0x02000386 RID: 902
		public enum skillNameType
		{
			// Token: 0x040024E9 RID: 9449
			UB,
			// Token: 0x040024EA RID: 9450
			[Description("技能1")]
			SKILL1,
			// Token: 0x040024EB RID: 9451
			[Description("技能2")]
			SKILL2,
			// Token: 0x040024EC RID: 9452
			[Description("普攻")]
			ATK
		}

		// Token: 0x02000387 RID: 903
		public enum skillType
		{
			// Token: 0x040024EE RID: 9454
			[Description("攻击类")]
			ATK,
			// Token: 0x040024EF RID: 9455
			[Description("减速类")]
			ChangeSpeed,
			// Token: 0x040024F0 RID: 9456
			[Description("咕咕咕")]
			KNOCK,
			// Token: 0x040024F1 RID: 9457
			[Description("DOT伤害")]
			SLIP_DAMAGE,
			// Token: 0x040024F2 RID: 9458
			[Description("致盲黑暗类")]
			BLIND
		}

		// Token: 0x02000388 RID: 904
		public enum ResultType
		{
			// Token: 0x040024F4 RID: 9460
			[Description("正常随机")]
			RANDOM,
			// Token: 0x040024F5 RID: 9461
			[Description("必中")]
			FORCE_ACC,
			// Token: 0x040024F6 RID: 9462
			[Description("必爆")]
			FORCE_CRI,
			// Token: 0x040024F7 RID: 9463
			[Description("必MISS")]
			FORCE_MISS,
			// Token: 0x040024F8 RID: 9464
			[Description("必不爆")]
			FORCE_NORMAL
		}
	}
}

namespace PCRCaculator.Guild
{
	// Token: 0x0200011E RID: 286
	public class GuildRandomData
	{
		// Token: 0x04000859 RID: 2137
		public string DataName = "默认世界线";

		// Token: 0x0400085A RID: 2138
		public bool UseFixedRandomSeed = true;

		// Token: 0x0400085B RID: 2139
		public int RandomSeed = 666;

		// Token: 0x0400085C RID: 2140
		public bool ForceNoCritical_player;

		// Token: 0x0400085D RID: 2141
		public bool ForceNoCritical_enemy;

		// Token: 0x0400085E RID: 2142
		public bool ForceIgnoreDodge_player;

		// Token: 0x0400085F RID: 2143
		public bool ForceIgnoreDodge_enemy;

		// Token: 0x04000860 RID: 2144
		public bool ForceCritical_player;

		// Token: 0x04000861 RID: 2145
		public List<GuildRandomSpecialData> randomSpecialDatas = new List<GuildRandomSpecialData>();
	}
}

namespace PCRCaculator
{
	// Token: 0x020000E2 RID: 226
	[Serializable]
	public class AddedPlayerData
	{
		// Token: 0x040005DB RID: 1499
		public string playerName = "未命名";

		// Token: 0x040005DC RID: 1500
		public int playerLevel = 100;

		// Token: 0x040005DD RID: 1501
		public int totalpoint;

		// Token: 0x040005DE RID: 1502
		public List<UnitData> playrCharacters = new List<UnitData>();
	}
}

namespace PCRCaculator
{
	// Token: 0x020000F8 RID: 248
	[Serializable]
	public class UnitData
	{
		// Token: 0x0400067D RID: 1661
		public int unitId;

		// Token: 0x0400067E RID: 1662
		public int level = 1;

		// Token: 0x0400067F RID: 1663
		public int rarity = 1;

		// Token: 0x04000680 RID: 1664
		public int love;

		// Token: 0x04000681 RID: 1665
		public int rank = 1;

		// Token: 0x04000682 RID: 1666
		public int[] equipLevel = new int[]
		{
			-1,
			-1,
			-1,
			-1,
			-1,
			-1
		};

		// Token: 0x04000683 RID: 1667
		public int[] skillLevel = new int[]
		{
			1,
			1,
			1,
			1
		};

		// Token: 0x04000684 RID: 1668
		public Dictionary<int, int> playLoveDic;

		// Token: 0x04000685 RID: 1669
		public int uniqueEqLv;

		// Token: 0x04000686 RID: 1670
		private string name = "";
	}
}

namespace PCRCaculator.Guild
{
	// Token: 0x0200011C RID: 284
	[Serializable]
	public class GuildPlayerGroupData
	{
		public AddedPlayerData playerData;
		// Token: 0x0400083E RID: 2110
		public List<List<float>> UBExecTimeData;

		// Token: 0x0400083F RID: 2111
		public bool useAutoMode;

		// Token: 0x04000840 RID: 2112
		public GuildRandomData timeLineData;

		// Token: 0x04000841 RID: 2113
		public int currentGuildMonth = 9;

		// Token: 0x04000842 RID: 2114
		public int currentGuildEnemyNum = 1;

		// Token: 0x04000843 RID: 2115
		public int currentTurn = 1;

		// Token: 0x04000844 RID: 2116
		public int selectedEnemyID;

		// Token: 0x04000845 RID: 2117
		public bool isViolent;

		// Token: 0x04000846 RID: 2118
		public bool usePlayerSettingHP;

		// Token: 0x04000847 RID: 2119
		public int playerSetingHP;

		// Token: 0x04000848 RID: 2120
		public bool useLogBarrier;

		// Token: 0x04000849 RID: 2121
		public bool isSpecialBoss;

		// Token: 0x0400084A RID: 2122
		public int specialBossID;

		// Token: 0x0400084B RID: 2123
		public int specialInputValue;
	}
}

namespace PCRCaculator.Guild
{
	// Token: 0x02000188 RID: 392
	public class UBDetail
	{
		// Token: 0x0400093E RID: 2366
		public bool isBossUB;

		// Token: 0x0400093F RID: 2367
		public UnitData unitData;

		// Token: 0x04000940 RID: 2368
		public int UBTime;

		// Token: 0x04000941 RID: 2369
		public int Damage;

		// Token: 0x04000942 RID: 2370
		public bool Critical;
	}
	public enum ActionState
	{
		IDLE = 0,
		ATK = 1,
		SKILL_1 = 2,
		SKILL = 3,
		WALK = 4,
		DAMAGE = 5,
		DIE = 6,
		GAME_START = 7
	}

	public struct UnitStateChangeData
	{
		public int currentFrameCount;
		public ActionState changStateFrom;
		public ActionState changStateTo;
		public string describe;
	}
	public class Timeline
    {
		public GuildTimelineData timeline;
		public Dictionary<int, List<UnitStateChangeData>> state;
	}
	// Token: 0x02000114 RID: 276
	public class GuildTimelineData
	{
		// Token: 0x040007AB RID: 1963
		public int currentRandomSeed;

		// Token: 0x040007AC RID: 1964
		public GuildPlayerGroupData playerGroupData;

		// Token: 0x040007B6 RID: 1974
		public int exceptDamage;

		// Token: 0x040007B7 RID: 1975
		public int backDamage;

		// Token: 0x040007B9 RID: 1977
		public string timeLineName;

	}
}
