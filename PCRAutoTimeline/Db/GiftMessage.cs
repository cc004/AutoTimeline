using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class GiftMessage
    {
        public long Id { get; set; }
        public string Discription { get; set; }
        public long Type1 { get; set; }
        public long Type2 { get; set; }
        public long Type3 { get; set; }
        public long Type4 { get; set; }
    }
}
