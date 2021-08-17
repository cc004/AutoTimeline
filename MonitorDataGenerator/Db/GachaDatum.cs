using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class GachaDatum
    {
        public long GachaId { get; set; }
        public string GachaName { get; set; }
        public string Description { get; set; }
        public string Description2 { get; set; }
        public string DescriptionSp { get; set; }
        public long GachaDetail { get; set; }
        public long GachaCostType { get; set; }
        public long Price { get; set; }
        public long FreeGachaType { get; set; }
        public long FreeGachaIntervalTime { get; set; }
        public long FreeGachaCount { get; set; }
        public long DiscountPrice { get; set; }
        public string GachaOdds { get; set; }
        public string GachaOddsStar2 { get; set; }
        public long GachaType { get; set; }
        public long MovieId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public long TicketId { get; set; }
        public long SpecialId { get; set; }
        public long ExchangeId { get; set; }
        public long TicketId10 { get; set; }
        public string RarityOdds { get; set; }
        public string CharaOddsStar1 { get; set; }
        public string CharaOddsStar2 { get; set; }
        public string CharaOddsStar3 { get; set; }
        public string Gacha10SpecialOddsStar1 { get; set; }
        public string Gacha10SpecialOddsStar2 { get; set; }
        public string Gacha10SpecialOddsStar3 { get; set; }
        public long PrizegachaId { get; set; }
        public long GachaBonusId { get; set; }
        public long GachaTimesLimit10 { get; set; }
    }
}
