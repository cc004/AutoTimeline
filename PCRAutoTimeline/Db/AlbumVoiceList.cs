using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class AlbumVoiceList
    {
        public long Id { get; set; }
        public long UnitId { get; set; }
        public string SheetId { get; set; }
        public string VoiceId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}
