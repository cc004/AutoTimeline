using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ActualUnitBackground
    {
        public long UnitId { get; set; }
        public string UnitName { get; set; }
        public long BgId { get; set; }
        public long FaceType { get; set; }
    }
}
