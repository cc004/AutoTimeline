using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class UnitBackground
    {
        public long UnitId { get; set; }
        public string UnitName { get; set; }
        public long BgId { get; set; }
        public string BgName { get; set; }
        public double Position { get; set; }
        public long FaceType { get; set; }
    }
}
