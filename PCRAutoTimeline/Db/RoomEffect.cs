using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomEffect
    {
        public long Id { get; set; }
        public long RewardGet { get; set; }
        public long Jukebox { get; set; }
        public long Nebbia { get; set; }
        public long Arcade { get; set; }
    }
}
