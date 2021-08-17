using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class NotifDatum
    {
        public long UnitId { get; set; }
        public long NotifType { get; set; }
        public string Comment { get; set; }
    }
}
