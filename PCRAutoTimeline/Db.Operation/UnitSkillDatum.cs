using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class UnitSkillDatum
    {
        public static UnitSkillDatum FromUnitId(long unit_id) =>
            MasterDataContext.Instance.UnitSkillData.Single(record => record.UnitId == unit_id);
    }
}
