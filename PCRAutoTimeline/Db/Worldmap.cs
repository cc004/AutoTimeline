using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class Worldmap
    {
        public long CourseId { get; set; }
        public string Name { get; set; }
        public long MapId { get; set; }
        public string SheetId { get; set; }
        public string QueId { get; set; }
        public long StartAreaId { get; set; }
        public long EndAreaId { get; set; }
    }
}
