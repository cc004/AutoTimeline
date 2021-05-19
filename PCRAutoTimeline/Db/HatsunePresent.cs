using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsunePresent
    {
        public long Id { get; set; }
        public long EventId { get; set; }
        public string DialogTitle { get; set; }
        public string DialogText { get; set; }
        public long ConditionQuestId { get; set; }
        public long ConditionBossId { get; set; }
        public long ConditionMissionId { get; set; }
        public long AdvId { get; set; }
        public long ItemType1 { get; set; }
        public long ItemId1 { get; set; }
        public long ItemNum1 { get; set; }
        public long ItemType2 { get; set; }
        public long ItemId2 { get; set; }
        public long ItemNum2 { get; set; }
        public long ItemType3 { get; set; }
        public long ItemId3 { get; set; }
        public long ItemNum3 { get; set; }
        public long ItemType4 { get; set; }
        public long ItemId4 { get; set; }
        public long ItemNum4 { get; set; }
        public long ItemType5 { get; set; }
        public long ItemId5 { get; set; }
        public long ItemNum5 { get; set; }
    }
}
