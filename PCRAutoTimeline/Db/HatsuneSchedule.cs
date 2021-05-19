using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneSchedule
    {
        public long EventId { get; set; }
        public string TeaserTime { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string CloseTime { get; set; }
        public long Background { get; set; }
        public string SheetId { get; set; }
        public string QueId { get; set; }
        public long BannerUnitId { get; set; }
        public string CountStartTime { get; set; }
        public long BackgroudSizeX { get; set; }
        public long BackgroudSizeY { get; set; }
        public long BackgroudPosX { get; set; }
        public long BackgroudPosY { get; set; }
        public long OriginalEventId { get; set; }
        public long SeriesEventId { get; set; }
    }
}
