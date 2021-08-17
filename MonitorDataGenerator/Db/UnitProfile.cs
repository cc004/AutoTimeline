using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class UnitProfile
    {
        public long UnitId { get; set; }
        public string UnitName { get; set; }
        public string Age { get; set; }
        public string Guild { get; set; }
        public string Race { get; set; }
        public string Height { get; set; }
        public string Weight { get; set; }
        public string BirthMonth { get; set; }
        public string BirthDay { get; set; }
        public string BloodType { get; set; }
        public string Favorite { get; set; }
        public string Voice { get; set; }
        public long VoiceId { get; set; }
        public string CatchCopy { get; set; }
        public string SelfText { get; set; }
        public string GuildId { get; set; }
    }
}
