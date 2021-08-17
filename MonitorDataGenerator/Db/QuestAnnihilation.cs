using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class QuestAnnihilation
    {
        public long SystemId { get; set; }
        public long QuestId { get; set; }
        public long EffectType { get; set; }
        public long QuestEffectPosition { get; set; }
    }
}
