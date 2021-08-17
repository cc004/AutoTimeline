using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ClanBattleOddsDatum
    {
        public long OddsGroupId { get; set; }
        public long TeamLevelFrom { get; set; }
        public long TeamLevelTo { get; set; }
        public string OddsCsv1 { get; set; }
        public string OddsCsv2 { get; set; }
        public string OddsCsv3 { get; set; }
        public string OddsCsv4 { get; set; }
        public string OddsCsv5 { get; set; }
        public string OddsCsv6 { get; set; }
        public string OddsCsv7 { get; set; }
        public string OddsCsv8 { get; set; }
        public string OddsCsv9 { get; set; }
        public string OddsCsv10 { get; set; }
    }
}
