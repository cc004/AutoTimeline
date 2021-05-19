using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ContentMapDatum
    {
        public long ContentMapId { get; set; }
        public string Name { get; set; }
        public long MapType { get; set; }
        public long AreaId { get; set; }
        public long ConditionQuestId { get; set; }
        public long QuestPositionX { get; set; }
        public long QuestPositionY { get; set; }
        public long IconId { get; set; }
        public long SystemId { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
    }
}
