using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class PctSystemFruit
    {
        public long Id { get; set; }
        public long LastTime { get; set; }
        public long Appearance { get; set; }
        public long BarSplit { get; set; }
        public long AppearanceCharaOdds { get; set; }
        public string AppearancePattern { get; set; }
        public long WaitTime { get; set; }
    }
}
