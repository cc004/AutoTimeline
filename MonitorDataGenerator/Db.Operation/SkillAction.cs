using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class SkillAction
    {
        public static SkillAction FromActionId(long action_id) =>
            MasterDataContext.Instance.SkillActions.SingleOrDefault(record => record.ActionId == action_id);
    }
}
