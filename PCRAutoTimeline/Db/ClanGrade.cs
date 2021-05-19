using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class ClanGrade
    {
        public long ClanGradeId { get; set; }
        public long RankFrom { get; set; }
        public long RankTo { get; set; }
    }
}
