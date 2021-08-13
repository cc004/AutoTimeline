using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class BgDatum
    {
        public string ViewName { get; set; }
        public long BgId { get; set; }
        public long EventId { get; set; }
    }
}
