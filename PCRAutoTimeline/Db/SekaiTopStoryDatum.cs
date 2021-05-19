using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SekaiTopStoryDatum
    {
        public long SekaiId { get; set; }
        public long StoryId { get; set; }
        public string BossTimeFrom { get; set; }
        public string BossTimeTo { get; set; }
    }
}
