using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class LoginBonusAdv
    {
        public long Id { get; set; }
        public long LoginBonusId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long CountKey { get; set; }
        public long AdvId { get; set; }
    }
}
