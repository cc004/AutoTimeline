using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneBoss
    {
        public long BossId { get; set; }
        public long EventId { get; set; }
        public long AreaId { get; set; }
        public long Difficulty { get; set; }
        public string QuestName { get; set; }
        public long PositionX { get; set; }
        public long PositionY { get; set; }
        public long BossPositionX { get; set; }
        public long BossPositionY { get; set; }
        public long ResultBossPositionY { get; set; }
        public long IconId { get; set; }
        public long UseTicketNum { get; set; }
        public long TeamExp { get; set; }
        public long UnitExp { get; set; }
        public long Love { get; set; }
        public long LimitTime { get; set; }
        public long DailyLimit { get; set; }
        public long ClearRewardGroup { get; set; }
        public long EventBossTreasureBoxId1 { get; set; }
        public long Background1 { get; set; }
        public long WaveGroupId1 { get; set; }
        public string WaveBgmSheetId1 { get; set; }
        public string WaveBgmQueId1 { get; set; }
        public long StoryIdWavestart1 { get; set; }
        public long StoryIdWaveend1 { get; set; }
        public long DetailBgId { get; set; }
        public long DetailBgPosition { get; set; }
        public double DetailBossBgSize { get; set; }
        public double DetailBossBgHeight { get; set; }
        public string RewardGoldCoefficient { get; set; }
        public long RewardGoldLimit { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public double MapPositionX { get; set; }
        public double MapPositionY { get; set; }
        public double MapSize { get; set; }
        public double DeatailAuraSize { get; set; }
        public double MapAuraSize { get; set; }
        public long OneblowCountOfSkipCondition { get; set; }
        public long RequiredSkipTicketCount { get; set; }
        public long RetireFlag { get; set; }
        public long DispOnBg { get; set; }
    }
}
