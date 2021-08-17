using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomSkinColor
    {
        public long SkinColorId { get; set; }
        public long ColorRed { get; set; }
        public long ColorGreen { get; set; }
        public long ColorBlue { get; set; }
    }
}
