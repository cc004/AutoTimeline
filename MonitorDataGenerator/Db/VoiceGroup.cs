using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class VoiceGroup
    {
        public long GroupId { get; set; }
        public string GroupIdComment { get; set; }
        public long GroupUnitId01 { get; set; }
        public long GroupUnitId02 { get; set; }
        public long GroupUnitId03 { get; set; }
        public long GroupUnitId04 { get; set; }
        public long GroupUnitId05 { get; set; }
    }
}
