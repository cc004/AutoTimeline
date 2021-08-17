using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class Minigame
    {
        public long Id { get; set; }
        public long MinigameSchemeId { get; set; }
        public long EventId { get; set; }
        public long ReleaseConditions1 { get; set; }
        public long ConditionsId1 { get; set; }
        public long FirstTimeStoryId { get; set; }
    }
}
