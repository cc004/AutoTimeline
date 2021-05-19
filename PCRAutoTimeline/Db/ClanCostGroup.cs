using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ClanCostGroup
    {
        public long Id { get; set; }
        public long CostGroupId { get; set; }
        public long Difficulty { get; set; }
        public long Count { get; set; }
        public long Cost { get; set; }
    }
}
