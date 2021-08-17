using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class Banner
    {
        public long BannerId { get; set; }
        public long Type { get; set; }
        public long SystemId { get; set; }
        public long Priority { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public long SubBannerId1 { get; set; }
        public long IsShowRoom { get; set; }
        public string Url { get; set; }
    }
}
