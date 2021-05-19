using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class QuestAreaDatum
    {
        public long AreaId { get; set; }
        public string AreaName { get; set; }
        public long MapType { get; set; }
        public string SheetId { get; set; }
        public string QueId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
