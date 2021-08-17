using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class DearSetting
    {
        public long EventId { get; set; }
        public string SystemName { get; set; }
        public long TutorialQuestId { get; set; }
        public long TutorialCharaIndex { get; set; }
        public long TutorialStoryId { get; set; }
    }
}
