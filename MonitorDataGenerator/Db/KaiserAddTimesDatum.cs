using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class KaiserAddTimesDatum
    {
        public long Id { get; set; }
        public long AddTimes { get; set; }
        public string AddTimesTime { get; set; }
        public long Duration { get; set; }
    }
}
