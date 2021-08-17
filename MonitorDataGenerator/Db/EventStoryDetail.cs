using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class EventStoryDetail
    {
        public long StoryId { get; set; }
        public long StoryGroupId { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public long VisibleType { get; set; }
        public long StoryEnd { get; set; }
        public long PreStoryId { get; set; }
        public long LoveLevel { get; set; }
        public long RequirementId { get; set; }
        public long UnlockQuestId { get; set; }
        public long StoryQuestId { get; set; }
        public long RewardType1 { get; set; }
        public long RewardId1 { get; set; }
        public long RewardValue1 { get; set; }
        public long RewardType2 { get; set; }
        public long RewardId2 { get; set; }
        public long RewardValue2 { get; set; }
        public long RewardType3 { get; set; }
        public long RewardId3 { get; set; }
        public long RewardValue3 { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
