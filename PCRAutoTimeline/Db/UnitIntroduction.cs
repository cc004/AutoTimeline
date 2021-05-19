using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class UnitIntroduction
    {
        public long Id { get; set; }
        public long GachaId { get; set; }
        public long IntroductionNumber { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long MaximumChunkSize1 { get; set; }
        public long MaximumChunkSizeLoop1 { get; set; }
        public long MaximumChunkSize2 { get; set; }
        public long MaximumChunkSizeLoop2 { get; set; }
        public long MaximumChunkSize3 { get; set; }
        public long MaximumChunkSizeLoop3 { get; set; }
    }
}
