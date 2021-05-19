using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class OmpDrama
    {
        public long CommandId { get; set; }
        public long DramaId { get; set; }
        public long CommandType { get; set; }
        public string Param01 { get; set; }
        public string Param02 { get; set; }
        public string Param03 { get; set; }
        public string Param04 { get; set; }
        public string Param05 { get; set; }
        public string Param06 { get; set; }
        public string Param07 { get; set; }
        public string Param08 { get; set; }
    }
}
