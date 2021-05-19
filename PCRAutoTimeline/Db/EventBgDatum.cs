using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class EventBgDatum
    {
        public long EventId { get; set; }
        public long BgId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
