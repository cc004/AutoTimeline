using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomCharacterPersonality
    {
        public long CharacterId { get; set; }
        public long PersonalityId { get; set; }
    }
}
