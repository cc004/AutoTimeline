using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomChatInfo
    {
        public long Id { get; set; }
        public long FormationId { get; set; }
        public long ScenarioId { get; set; }
    }
}
