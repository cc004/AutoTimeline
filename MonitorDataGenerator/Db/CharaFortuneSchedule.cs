using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class CharaFortuneSchedule
    {
        public long FortuneId { get; set; }
        public string Name { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
