using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ContentReleaseDatum
    {
        public long SystemId { get; set; }
        public long TeamLevel { get; set; }
        public long StoryId { get; set; }
        public long QuestId { get; set; }
        public string Dialog { get; set; }
    }
}
