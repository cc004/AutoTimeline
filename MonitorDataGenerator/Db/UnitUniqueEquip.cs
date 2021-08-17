using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class UnitUniqueEquip
    {
        public long UnitId { get; set; }
        public long EquipSlot { get; set; }
        public long EquipId { get; set; }
    }
}
