using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SrtPanel
    {
        public long ReadingId { get; set; }
        public string Reading { get; set; }
        public long ReadType { get; set; }
        public long PanelId { get; set; }
        public string DetailText { get; set; }
    }
}
