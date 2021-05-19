using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class GlossaryDetail
    {
        public long GlossaryId { get; set; }
        public long GlossaryCategoryId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public long UnlockStoryId { get; set; }
        public long CategoryType { get; set; }
        public long DispOrder { get; set; }
    }
}
