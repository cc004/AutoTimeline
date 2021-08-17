using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneDiarySetting
    {
        public long EventId { get; set; }
        public string BgmSheetName { get; set; }
        public string BgmCueName { get; set; }
    }
}
