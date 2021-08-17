using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class AlbumProductionList
    {
        public long Id { get; set; }
        public long UnitId { get; set; }
        public long Type { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
