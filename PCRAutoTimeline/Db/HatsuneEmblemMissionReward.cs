using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneEmblemMissionReward
    {
        public long Id { get; set; }
        public long MissionRewardId { get; set; }
        public long RewardType { get; set; }
        public long RewardId { get; set; }
        public long RewardNum { get; set; }
        public long IconType { get; set; }
    }
}
