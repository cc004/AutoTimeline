using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ClanBattleSchedule
    {
        public long ClanBattleId { get; set; }
        public long ReleaseMonth { get; set; }
        public long LastClanBattleId { get; set; }
        public long PointPerStamina { get; set; }
        public long CostGroupId { get; set; }
        public long CostGroupIdS { get; set; }
        public string MapBgm { get; set; }
        public long ResourceId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
