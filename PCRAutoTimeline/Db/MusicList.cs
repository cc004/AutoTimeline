using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class MusicList
    {
        public long MusicId { get; set; }
        public string ListName { get; set; }
        public double FontSize { get; set; }
        public string PreShopStart { get; set; }
        public string ShopStart { get; set; }
        public string ShopEnd { get; set; }
        public long StoryId { get; set; }
        public long CostItemNum { get; set; }
        public long Sort { get; set; }
        public string Kana { get; set; }
        public string IosUrl { get; set; }
        public string AndroidUrl { get; set; }
        public string DmmUrl { get; set; }
    }
}
