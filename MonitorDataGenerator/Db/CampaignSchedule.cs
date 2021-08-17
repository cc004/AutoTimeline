using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class CampaignSchedule
    {
        public long Id { get; set; }
        public long CampaignCategory { get; set; }
        public double Value { get; set; }
        public long SystemId { get; set; }
        public long IconImage { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long LevelId { get; set; }
    }
}
