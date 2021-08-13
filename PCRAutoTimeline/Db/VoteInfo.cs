using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class VoteInfo
    {
        public long VoteId { get; set; }
        public long VoteHelpIndex { get; set; }
        public string VoteTitle { get; set; }
        public string VoteHelp { get; set; }
    }
}
