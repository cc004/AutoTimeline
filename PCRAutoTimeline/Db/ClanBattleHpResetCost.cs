using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ClanBattleHpResetCost
    {
        public long Id { get; set; }
        public long ResetCountFrom { get; set; }
        public long ResetCountTo { get; set; }
        public long CostNum { get; set; }
    }
}
