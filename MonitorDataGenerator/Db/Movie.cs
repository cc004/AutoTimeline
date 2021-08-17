using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class Movie
    {
        public long MovieId { get; set; }
        public long StoryGroupId { get; set; }
        public long StoryId { get; set; }
        public string BgmId { get; set; }
        public string SeId { get; set; }
    }
}
