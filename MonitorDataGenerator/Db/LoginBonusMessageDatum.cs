using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class LoginBonusMessageDatum
    {
        public long Id { get; set; }
        public long LoginBonusId { get; set; }
        public long Type { get; set; }
        public long DayCount { get; set; }
        public long LuckPattern { get; set; }
        public long Rate { get; set; }
        public long CharacterId { get; set; }
        public string CharacterName { get; set; }
        public string Message { get; set; }
        public long VoiceId { get; set; }
    }
}
