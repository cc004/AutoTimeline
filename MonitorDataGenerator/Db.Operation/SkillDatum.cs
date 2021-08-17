using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SkillDatum
    {
        public long[] Actions => new long[]
        {
            Action1, Action2, Action3, Action4, Action5, Action6, Action7
        };

        public long[] DependActions => new long[]
        {
            DependAction1, DependAction2, DependAction3, DependAction4, DependAction5, DependAction6, DependAction7
        };

        public static SkillDatum FromSkillId(long skill_id) =>
            MasterDataContext.Instance.SkillData.Single(record => record.SkillId == skill_id);
    }
}
