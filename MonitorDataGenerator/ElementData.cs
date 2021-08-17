

using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace Elements
{
    public class UnitActionControllerData
    {
        public PCRCaculator.Battle.ActionParameterOnPrefabDetail AttackDetail;
        public bool UseDefaultDelay;
        public PCRCaculator.Battle.Skill Attack;
        public List<PCRCaculator.Battle.Skill> UnionBurstList;
        public List<PCRCaculator.Battle.Skill> MainSkillList;
        public List<PCRCaculator.Battle.Skill> SpecialSkillList;
        public List<PCRCaculator.Battle.Skill> UnionBurstEvolutionList;
        public List<PCRCaculator.Battle.Skill> MainSkillEvolutionList;
        public PCRCaculator.Battle.Skill Annihilation;

        public UnitActionControllerData(PCRCaculator.Battle.ActionParameterOnPrefabDetail attackDetail, bool useDefaultDelay, PCRCaculator.Battle.Skill attack, List<PCRCaculator.Battle.Skill> unionBurstList, List<PCRCaculator.Battle.Skill> mainSkillList, List<PCRCaculator.Battle.Skill> specialSkillList, List<PCRCaculator.Battle.Skill> unionBurstEvolutionList, List<PCRCaculator.Battle.Skill> mainSkillEvolutionList, PCRCaculator.Battle.Skill annihilation)
        {
            AttackDetail = attackDetail;
            UseDefaultDelay = useDefaultDelay;
            Attack = attack;
            UnionBurstList = unionBurstList;
            MainSkillList = mainSkillList;
            SpecialSkillList = specialSkillList;
            UnionBurstEvolutionList = unionBurstEvolutionList;
            MainSkillEvolutionList = mainSkillEvolutionList;
            Annihilation = annihilation;
        }
    }
    public class UnitPrefabData
    {
        public Elements.UnitActionControllerData UnitActionControllerData;
        public Dictionary<string, List<Elements.FirearmCtrlData>> unitFirearmDatas;

        private static readonly Dictionary<int, UnitPrefabData> prefabs = new Dictionary<int, UnitPrefabData>();

        public static UnitPrefabData Get(int unit_id)
        {
            if (!prefabs.ContainsKey(unit_id))
            {
                prefabs[unit_id] = JsonConvert.DeserializeObject<UnitPrefabData>(File.ReadAllText($"Data/unitPrefabDatas/UNIT_{unit_id}.json"));
            }
            return prefabs[unit_id];
        }
    }
    public class FirearmCtrlData
    {
        public float HitDelay = 0.5f;
        public float MoveRate = 1;
        public float duration;
        public PCRCaculator.Battle.eMoveTypes MoveType = PCRCaculator.Battle.eMoveTypes.LINEAR;
        public float startRotate;
        public float endRotate;
        //public Bounds ColliderBox;
        public float[] ColliderBoxCentre = new float[3] { 0, 0, 0 };
        public float[] ColliderBoxSize = new float[3] { 0.2f, 0.2f, 0 };

        public bool ignoreFirearm = false;
        public float fixedExecTime = 0;


    }
}

namespace PCRCaculator.Battle
{
    public enum eMoveTypes
    {
        LINEAR = 0,
        NONE = 1,
        PARABORIC = 2,
        PARABORIC_ROTATE = 3,
        HORIZONTAL = 4,
        INVALID_VALUE = -1
    }
    public enum eAnimationType
    {
        run_start = 0, idle = 1, attack = 2, attack_skipQuest = 3, damage = 4, die = 5, run = 6, walk = 7, standBy = 8, multi_standBy = 9,
        joy_long = 10, joy_long_return = 11, joy_short = 12, joy_short_return = 13,

        skill0 = 20, skill0_1 = 21, skill0_2 = 22, skill0_3 = 23, skill0_4 = 24,
        skill1 = 30, skill1_1 = 31, skill1_2 = 32, skill1_3 = 33, skill1_4 = 34,
        skill2 = 40, skill2_1 = 41, skill2_2 = 42, skill2_3 = 43, skill2_4 = 44

    }
    public class ActionExecTime
    {
        public float Time;
        public eDamageEffectType DamageNumType;
        public float Weight = 1f;
        public float DamageNumScale = 1f;
    }
    public class ActionExecTimeCombo
    {
        public float StartTime;
        public float OffsetTime;
        public float Weight = 1f;
        public int Count;
        public eComboInterporationType InterporationType;
    }

    public class ActionParameterOnPrefabDetail
    {
        //public Data data;
        public bool Visible;
        public List<ActionExecTime> execTime;
        public List<ActionExecTime> ExecTimeForPrefab;//ExecTimeForPrefab
        public List<ActionExecTimeCombo> ExecTimeCombo = new List<ActionExecTimeCombo>();
        public int ActionId;
        //public List<NormalSkillEffect> ActionEffectList;

        public List<ActionExecTime> ExecTime { get => execTime; set => execTime = value; }
        /*public sealed class Data
        {
            public bool Visible;
            public List<ActionExecTime> execTime;
            public List<ActionExecTime> ExecTimeForPrefab;//ExecTimeForPrefab
            public List<ActionExecTimeCombo> ExecTimeCombo;
            public int ActionId;
            //public List<NormalSkillEffect> ActionEffectList;

        }*/
    }
    public class ActionParameterOnPrefab
    {
        //public Data data;
        public bool Visible;
        public eActionType ActionType;
        public List<ActionParameterOnPrefabDetail> Details;
        //public AnimationCurve KnockAnimationCurve
        //public AnimationCurve KnockDownAnimationCurve
        public eEffectType EffectType;

        /*public sealed class Data
        {
            public bool Visible;
            public eActionType ActionType;
            public List<ActionParameterOnPerferbDetail> Details;
            //public AnimationCurve KnockAnimationCurve
            //public AnimationCurve KnockDownAnimationCurve
            public eEffectType EffectType;

        }*/
    }
    public class Skill
    {
        public bool IsPrincessForm;
        public List<ActionParameterOnPrefab> ActionParametersOnPrefab = new List<ActionParameterOnPrefab>();
        public bool ForcePlayNoTarget;
        public int ParameterTarget;
        private float castTime;
        public int SkillId;
        public float skillAreaWidth;
        public eAnimationType animationId;
        public bool Cancel;
        private int skillNum;
        private List<int> hasParentIndexes;
        public float BlackOutTime;
        public bool BlackoutEndtWithMotion;
        public bool ForceComboDamage;
        public float CutInMovieFadeStartTime;
        public float CutInMovieFadeDurationTime;
        public float CutInSkipTime;//如果IsPrincessForm则该项为0
        public int Level;
        public string SkillName = "未命名";
        private bool isModeChange;
        public eSkillMotionType SkillMotionType;
        public bool TrackDamageNum = true;
        public eWeaponSeType WeaponType;
        public bool PauseStopState;
        public List<int> BranchIds = new List<int>();


    }
}