using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class CampaignMissionSchedule
    {
        public long CampaignId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CloseTime { get; set; }
    }
}
