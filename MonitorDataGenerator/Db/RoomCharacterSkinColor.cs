using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomCharacterSkinColor
    {
        public long CharacterId { get; set; }
        public long SkinColorId { get; set; }
    }
}
