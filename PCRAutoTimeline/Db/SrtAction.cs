using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SrtAction
    {
        public string ActionName { get; set; }
        public string InoriAction { get; set; }
        public string DragonAction { get; set; }
        public string KayaAction { get; set; }
        public string HomareAction { get; set; }
        public long TalkTextType { get; set; }
        public string TalkText { get; set; }
        public string VoiceList { get; set; }
    }
}
