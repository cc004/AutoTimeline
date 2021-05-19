using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ProfileFrame
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public long Type { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long DispOrder { get; set; }
    }
}
