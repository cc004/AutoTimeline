using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class UnlockSkillDatum
    {
        public long PromotionLevel { get; set; }
        public long UnlockSkill { get; set; }
    }
}
