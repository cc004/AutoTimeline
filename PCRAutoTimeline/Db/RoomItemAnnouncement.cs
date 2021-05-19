using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class RoomItemAnnouncement
    {
        public long Id { get; set; }
        public string AnnouncementStart { get; set; }
        public string AnnouncementEnd { get; set; }
        public string AnnouncementText { get; set; }
    }
}
