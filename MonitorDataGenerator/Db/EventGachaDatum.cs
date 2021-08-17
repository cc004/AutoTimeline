using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class EventGachaDatum
    {
        public long GachaId { get; set; }
        public long EventId { get; set; }
        public string GachaName { get; set; }
        public long ItemType { get; set; }
        public long ItemId { get; set; }
        public long Cost { get; set; }
        public long RepeatStep { get; set; }
    }
}
