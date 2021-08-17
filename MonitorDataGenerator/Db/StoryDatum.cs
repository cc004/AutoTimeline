using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class StoryDatum
    {
        public long StoryGroupId { get; set; }
        public long StoryType { get; set; }
        public long Value { get; set; }
        public string Title { get; set; }
        public long ThumbnailId { get; set; }
        public long DispOrder { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long Order { get; set; }
        public long ConditionFreeFlag { get; set; }
    }
}
