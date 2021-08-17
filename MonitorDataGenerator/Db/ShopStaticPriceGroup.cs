using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ShopStaticPriceGroup
    {
        public long Id { get; set; }
        public long PriceGroupId { get; set; }
        public long BuyCountFrom { get; set; }
        public long BuyCountTo { get; set; }
        public long Count { get; set; }
    }
}
