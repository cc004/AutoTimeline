using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneBattleMissionDatum
    {
        public long MissionId { get; set; }
        public long DispGroup { get; set; }
        public long CategoryIcon { get; set; }
        public string Description { get; set; }
        public long MissionCondition { get; set; }
        public long? ConditionValue1 { get; set; }
        public long? ConditionValue2 { get; set; }
        public long? ConditionValue3 { get; set; }
        public long? ConditionValue4 { get; set; }
        public long? ConditionValue5 { get; set; }
        public long? ConditionValue6 { get; set; }
        public long? ConditionValue7 { get; set; }
        public long? ConditionValue8 { get; set; }
        public long? ConditionValue9 { get; set; }
        public long? ConditionValue10 { get; set; }
        public long ConditionNum { get; set; }
        public long MissionRewardId { get; set; }
        public long? SystemId { get; set; }
        public long EventId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
