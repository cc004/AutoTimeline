using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomItem
    {
        public long Id { get; set; }
        public long ItemType { get; set; }
        public long Category { get; set; }
        public string Name { get; set; }
        public long MaxLevel { get; set; }
        public long EnableRemove { get; set; }
        public long MaxPossessionNum { get; set; }
        public long EffectId1 { get; set; }
        public string ShopStart { get; set; }
        public string ShopEnd { get; set; }
        public string ShopNewDispEnd { get; set; }
        public long CostItemNum { get; set; }
        public long ShopOpenType { get; set; }
        public long ShopOpenId { get; set; }
        public long ShopOpenValue { get; set; }
        public long SoldPrice { get; set; }
        public long Sort { get; set; }
        public long ConditionQuestId { get; set; }
    }
}
