using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class PctTapSpeed
    {
        public long Id { get; set; }
        public long ComboCount { get; set; }
        public long SpeedMagnification { get; set; }
    }
}
