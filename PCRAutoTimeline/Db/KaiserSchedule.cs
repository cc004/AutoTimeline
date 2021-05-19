using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class KaiserSchedule
    {
        public long Id { get; set; }
        public string TeaserTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CountStartTime { get; set; }
        public string CloseTime { get; set; }
        public long StoryId { get; set; }
        public long CloseStoryConditionId { get; set; }
        public long CloseStoryId { get; set; }
        public string TopBgm { get; set; }
        public string TopBg { get; set; }
        public string AfterBgm { get; set; }
        public string AfterBg { get; set; }
    }
}
