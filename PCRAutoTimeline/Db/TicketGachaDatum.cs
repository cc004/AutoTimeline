using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class TicketGachaDatum
    {
        public long GachaId { get; set; }
        public string GachaName { get; set; }
        public long GachaType { get; set; }
        public long TicketId { get; set; }
        public long GachaTimes { get; set; }
        public long GachaDetail { get; set; }
        public string GuaranteeRarity { get; set; }
        public string RarityOdds { get; set; }
        public string CharaOddsStar1 { get; set; }
        public string CharaOddsStar2 { get; set; }
        public string CharaOddsStar3 { get; set; }
        public long StagingType { get; set; }
    }
}
