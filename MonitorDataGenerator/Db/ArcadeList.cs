using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ArcadeList
    {
        public long ArcadeId { get; set; }
        public string Title { get; set; }
        public string StartTime { get; set; }
        public long Price { get; set; }
        public string SheetId { get; set; }
        public string CueId { get; set; }
        public long WhereType { get; set; }
        public string Description { get; set; }
    }
}
