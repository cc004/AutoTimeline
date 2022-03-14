using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCRAutoTimeline.Models
{
    public enum ActionState
    {
        IDLE = 0, ATK, SKILL_1, SKILL,
        WALK, DAMAGE, SUMMON, DIE, GAME_START,
        LOSE
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct UnitCtrl
    {
        [FieldOffset(0x518)]
        public ObscuredFloat tp;
        [FieldOffset(0x3AC)]
        public ObscuredLong hp;
        [FieldOffset(0x3C0)]
        public ObscuredLong maxHp;
        [FieldOffset(0x3A0)]
        public ObscuredInt level;
        [FieldOffset(0x3EC)]
        public ObscuredInt def;
        [FieldOffset(0x3F8)]
        public ObscuredInt magicDef;
        [FieldOffset(0x41C)]
        public ObscuredInt physicalCritical;
        [FieldOffset(0x428)]
        public ObscuredInt magicCritical;
        [FieldOffset(0x18C)]
        public ActionState currentState;
        [FieldOffset(0x194)]
        public ObscuredFloat castTimer;
        [FieldOffset(0x1d8)]
        public int attackPatternIndex;
        [FieldOffset(0x1dc)]
        public bool attackPatternIsLoop;
        [FieldOffset(0x1e0)]
        public int currentActionPatternId;
        [FieldOffset(0x108)]
        public int currentSkill;
        [FieldOffset(0x24c)]
        public int unitId;
        [FieldOffset(0x250)]
        public int prefabId;
        [FieldOffset(0x25c)]
        public int rarity;
        [FieldOffset(0x264)]
        public int promotion;
    }
}
