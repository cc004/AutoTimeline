using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneUnlockStoryCondition
    {
        public long StoryId { get; set; }
        public long EventId { get; set; }
        public long ConditionEntry { get; set; }
        public long ConditionQuestId { get; set; }
        public long ConditionBossId { get; set; }
        public long ConditionMissionId { get; set; }
        public string ConditionTime { get; set; }
    }
}
