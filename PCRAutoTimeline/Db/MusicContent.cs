using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class MusicContent
    {
        public long MusicId { get; set; }
        public string Name { get; set; }
        public string TotalPlayingTime { get; set; }
        public string ListenStartTime { get; set; }
        public string Detail { get; set; }
        public string SheetId { get; set; }
        public string CueId { get; set; }
    }
}
