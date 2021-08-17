using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class PctComboCoefficient
    {
        public long Id { get; set; }
        public long ComboMin { get; set; }
        public long ComboMax { get; set; }
        public long ComboCoefficient { get; set; }
    }
}
