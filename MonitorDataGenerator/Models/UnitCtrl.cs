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
        [FieldOffset(0x4e0)]
        public ObscuredFloat tp;
        [FieldOffset(0x390)]
        public ObscuredLong hp;
        [FieldOffset(0x3a4)]
        public ObscuredLong maxHp;
        [FieldOffset(0x384)]
        public ObscuredInt level;
        [FieldOffset(0x3d0)]
        public ObscuredInt def;
        [FieldOffset(0x3dc)]
        public ObscuredInt magicDef;
        [FieldOffset(0x400)]
        public ObscuredInt physicalCritical;
        [FieldOffset(0x40c)]
        public ObscuredInt magicCritical;
        [FieldOffset(0x18c)]
        public ActionState currentState;
        [FieldOffset(0x194)]
        public ObscuredFloat castTimer;
        [FieldOffset(0x1d8)]
        public int attackPatternIndex;
        [FieldOffset(0x1dc)]
        public bool attackPatternIsLoop;
        [FieldOffset(0x1e0)]
        public int currentActionPatternId;
    }
}
