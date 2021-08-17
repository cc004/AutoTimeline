using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class CharaETicketDatum
    {
        public long TicketId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long JewelStoreId { get; set; }
    }
}
