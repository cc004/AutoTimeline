using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class TowerAreaDatum
    {
        public long TowerAreaId { get; set; }
        public long MaxFloorNum { get; set; }
        public long AreaBg { get; set; }
        public string TowerBgm { get; set; }
        public long CloisterQuestId { get; set; }
    }
}
