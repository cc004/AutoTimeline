using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class GachaExchangeLineup
    {
        public long Id { get; set; }
        public long ExchangeId { get; set; }
        public long UnitId { get; set; }
        public long Rarity { get; set; }
        public long GachaBonusId { get; set; }
    }
}
