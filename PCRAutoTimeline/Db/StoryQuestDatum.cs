using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class StoryQuestDatum
    {
        public long StoryQuestId { get; set; }
        public long StoryId { get; set; }
        public string QuestName { get; set; }
        public long LimitTime { get; set; }
        public long Background1 { get; set; }
        public long WaveGroupId1 { get; set; }
        public string WaveBgmSheetId1 { get; set; }
        public string WaveBgmQueId1 { get; set; }
        public long Background2 { get; set; }
        public long WaveGroupId2 { get; set; }
        public string WaveBgmSheetId2 { get; set; }
        public string WaveBgmQueId2 { get; set; }
        public long Background3 { get; set; }
        public long WaveGroupId3 { get; set; }
        public string WaveBgmSheetId3 { get; set; }
        public string WaveBgmQueId3 { get; set; }
        public long GuestUnit1 { get; set; }
        public long GuestUnit2 { get; set; }
        public long GuestUnit3 { get; set; }
        public long GuestUnit4 { get; set; }
        public long GuestUnit5 { get; set; }
    }
}
