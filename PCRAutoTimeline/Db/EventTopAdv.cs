using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class EventTopAdv
    {
        public long EventTopAdvId { get; set; }
        public long EventId { get; set; }
        public long Type { get; set; }
        public long Value1 { get; set; }
        public long Value2 { get; set; }
        public long Value3 { get; set; }
        public long StoryId { get; set; }
        public long CharacterId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
