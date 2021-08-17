using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class TowerSchedule
    {
        public long TowerScheduleId { get; set; }
        public long MaxTowerAreaId { get; set; }
        public long OpeningStoryId { get; set; }
        public string CountStartTime { get; set; }
        public string RecoveryDisableTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
