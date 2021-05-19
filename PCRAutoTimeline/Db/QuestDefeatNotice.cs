using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class QuestDefeatNotice
    {
        public long Id { get; set; }
        public long ImageId { get; set; }
        public long RequiredTeamLevel { get; set; }
        public long RequiredQuestId { get; set; }
    }
}
