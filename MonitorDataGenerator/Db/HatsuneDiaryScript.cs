using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class HatsuneDiaryScript
    {
        public long Id { get; set; }
        public long DiaryId { get; set; }
        public long SeqNum { get; set; }
        public long Type { get; set; }
        public string DiaryText { get; set; }
        public long TextAnimationSpeed { get; set; }
        public string SheetName { get; set; }
        public string CueName { get; set; }
        public long Command { get; set; }
        public double CommandParam { get; set; }
    }
}
