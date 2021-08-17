using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class AilmentDatum
    {
        public long AilmentId { get; set; }
        public long AilmentAction { get; set; }
        public long AilmentDetail1 { get; set; }
        public string AilmentName { get; set; }
    }
}
