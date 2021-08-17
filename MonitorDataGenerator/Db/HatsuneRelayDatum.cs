using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneRelayDatum
    {
        public long RelayStoryId { get; set; }
        public long IsEnableRead { get; set; }
        public long ConditionQuestId { get; set; }
        public long StorySeq { get; set; }
        public string SubTitle { get; set; }
    }
}
