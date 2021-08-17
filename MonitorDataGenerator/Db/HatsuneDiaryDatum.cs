using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneDiaryDatum
    {
        public long DiaryId { get; set; }
        public long ContentsType { get; set; }
        public long DiaryDate { get; set; }
        public string SubTitle { get; set; }
        public string ForcedReleaseTime { get; set; }
        public string ConditionTime { get; set; }
        public long ConditionStoryId { get; set; }
        public long ConditionBossCount { get; set; }
    }
}
