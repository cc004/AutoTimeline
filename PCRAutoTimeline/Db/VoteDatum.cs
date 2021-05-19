using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class VoteDatum
    {
        public long VoteId { get; set; }
        public string VoteStartTime { get; set; }
        public string VoteEndTime { get; set; }
        public string ResultStartTime { get; set; }
        public string ResultEndTime { get; set; }
        public long StartStoryId { get; set; }
        public long ResultStoryId { get; set; }
    }
}
