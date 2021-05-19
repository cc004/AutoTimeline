using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class Battlelog
    {
        public string Team { get; set; }
        public string Boss { get; set; }
        public string Lap { get; set; }
        public long? Damage { get; set; }
        public string Log { get; set; }
        public long? Frame { get; set; }
        public long Id { get; set; }
    }
}
