using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneMapEvent
    {
        public long Id { get; set; }
        public long TargetEventId { get; set; }
        public long EventType { get; set; }
        public long ConditionId { get; set; }
        public long Param1 { get; set; }
        public long Param2 { get; set; }
    }
}
