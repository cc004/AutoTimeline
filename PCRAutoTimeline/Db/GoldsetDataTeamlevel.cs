using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class GoldsetDataTeamlevel
    {
        public long Id { get; set; }
        public long TeamLevel { get; set; }
        public long InitialGetGoldCount { get; set; }
    }
}
