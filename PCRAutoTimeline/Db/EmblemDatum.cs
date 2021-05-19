using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class EmblemDatum
    {
        public long EmblemId { get; set; }
        public long DispOder { get; set; }
        public long Type { get; set; }
        public string EmblemName { get; set; }
        public long DescriptionMissionId { get; set; }
        public long EventEmblem { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
