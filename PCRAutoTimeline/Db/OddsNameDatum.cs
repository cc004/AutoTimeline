using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class OddsNameDatum
    {
        public long Id { get; set; }
        public string OddsFile { get; set; }
        public string Name { get; set; }
        public long IconType { get; set; }
        public string Description { get; set; }
    }
}
