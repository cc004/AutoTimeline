using System;
using System.Collections.Generic;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class EnemyMPart
    {
        public long EnemyId { get; set; }
        public string Name { get; set; }
        public long ChildEnemyParameter1 { get; set; }
        public long ChildEnemyParameter2 { get; set; }
        public long ChildEnemyParameter3 { get; set; }
        public long ChildEnemyParameter4 { get; set; }
        public long ChildEnemyParameter5 { get; set; }
    }
}
