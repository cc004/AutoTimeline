using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SkillCost
    {
        public long TargetLevel { get; set; }
        public long Cost { get; set; }
    }
}
