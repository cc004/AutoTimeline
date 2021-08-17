using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomSetup
    {
        public long RoomItemId { get; set; }
        public long GridHeight { get; set; }
        public long GridWidth { get; set; }
        public long UnitId { get; set; }
    }
}
