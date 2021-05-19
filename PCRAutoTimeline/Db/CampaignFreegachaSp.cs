using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class CampaignFreegachaSp
    {
        public long CampaignId { get; set; }
        public long MaxExecCount { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
