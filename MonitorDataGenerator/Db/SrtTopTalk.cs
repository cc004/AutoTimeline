using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SrtTopTalk
    {
        public long Id { get; set; }
        public long TalkId { get; set; }
        public long CharaIndex { get; set; }
        public string TalkText { get; set; }
        public string SheetName { get; set; }
        public string CueName { get; set; }
        public long Direction { get; set; }
    }
}
