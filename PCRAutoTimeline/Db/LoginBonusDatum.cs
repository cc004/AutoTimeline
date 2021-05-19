using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class LoginBonusDatum
    {
        public long LoginBonusId { get; set; }
        public string Name { get; set; }
        public long LoginBonusType { get; set; }
        public long CountNum { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long BgId { get; set; }
        public long StampId { get; set; }
        public long OddsGroupId { get; set; }
        public long AdvPlayType { get; set; }
        public long CountType { get; set; }
    }
}
