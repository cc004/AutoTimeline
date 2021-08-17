using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PCRApi.Models.Db
{
    public class MasterDataContext
    {
        [ThreadStatic]
        private static masterContext instance;

        public static masterContext Instance
        {
            get
            {
                if (instance == null) instance = new masterContext();
                return instance;
            }
        }

        [ThreadStatic]
        private static battlelogContext battlelog;

        public static battlelogContext BattleLog
        {
            get
            {
                if (battlelog == null) battlelog = new battlelogContext();
                return battlelog;
            }
        }
    }
}
