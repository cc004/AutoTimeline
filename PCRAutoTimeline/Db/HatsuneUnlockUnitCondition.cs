using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneUnlockUnitCondition
    {
        public long Id { get; set; }
        public long UnitId { get; set; }
        public long EventId { get; set; }
        public long ConditionMissionId { get; set; }
        public string TopDescription { get; set; }
        public string Description1 { get; set; }
        public string Description2 { get; set; }
    }
}
