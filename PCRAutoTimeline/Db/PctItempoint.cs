using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class PctItempoint
    {
        public long Id { get; set; }
        public long ItemId { get; set; }
        public long PctPointCoefficient { get; set; }
    }
}
