using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class GlobalDatum
    {
        public string KeyName { get; set; }
        public long Value { get; set; }
        public string Desc { get; set; }
    }
}
