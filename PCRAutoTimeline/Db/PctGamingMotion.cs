using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class PctGamingMotion
    {
        public long MotionId { get; set; }
        public long PerfectCount { get; set; }
        public long GoodCount { get; set; }
        public long NiceCount { get; set; }
        public long Point { get; set; }
    }
}
