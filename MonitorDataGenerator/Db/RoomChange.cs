using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomChange
    {
        public long RoomItemId { get; set; }
        public long ChangeId { get; set; }
        public string ChangeStart { get; set; }
        public string ChangeEnd { get; set; }
    }
}
