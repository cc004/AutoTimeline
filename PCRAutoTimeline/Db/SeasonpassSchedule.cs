using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SeasonpassSchedule
    {
        public long SeasonId { get; set; }
        public string Name { get; set; }
        public long KeyJewelId { get; set; }
        public long PointMax { get; set; }
        public long PointPrice { get; set; }
        public long PointChangeType { get; set; }
        public long RewardId { get; set; }
        public long Proportion { get; set; }
        public string StartTime { get; set; }
        public string LimitTime { get; set; }
        public string EndTime { get; set; }
    }
}
