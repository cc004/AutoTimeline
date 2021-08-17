using Elements;
using PCRApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.IO;

namespace UnitFrameData
{

        public class UnitData
        {
                public int prefab;
                public readonly Dictionary<long, long> exectime = new Dictionary<long, long>();
                public readonly Dictionary<long, long[]> actions = new Dictionary<long, long[]>();


                public void Initialize()
                {
                var prefabdata = UnitPrefabData.Get(prefab).UnitActionControllerData;
                var exectimes = new Dictionary<long, int>();
                if (prefab != 106101)
                {
                    exectimes = prefabdata.MainSkillEvolutionList.Concat(prefabdata.MainSkillList).Concat(prefabdata.SpecialSkillList)
                    .SelectMany(skill => skill.ActionParametersOnPrefab)
                    .Where(prefab => prefab.Visible)
                    .SelectMany(prefab => prefab.Details)
                    .Where(det => det.Visible && det.ActionId > 0)
                    .ToDictionary(ap => (long)ap.ActionId, ap => ap.ExecTimeForPrefab.Select(ex => ex.Time).Count() == 0 ? 0 : (1 + (int)Math.Ceiling(60 * ap.ExecTimeForPrefab.Select(ex => ex.Time).Max())));

                }
                else 
                {
                exectimes = prefabdata.MainSkillList.Concat(prefabdata.SpecialSkillList)
               .SelectMany(skill => skill.ActionParametersOnPrefab)
               .Where(prefab => prefab.Visible)
               .SelectMany(prefab => prefab.Details)
               .Where(det => det.Visible && det.ActionId > 0)
               .ToDictionary(ap => (long)ap.ActionId, ap => ap.ExecTimeForPrefab.Select(ex => ex.Time).Count() == 0 ? 0 : (1 + (int)Math.Ceiling(60 * ap.ExecTimeForPrefab.Select(ex => ex.Time).Max())));
                }
                Dictionary<long, long> dep = null; 
                long getexec(long a)
                {
                    if (exectime.TryGetValue(a, out var res)) return res;
                    var b = new long();
                    if (dep.ContainsKey(a)) { b = dep[a]; } else { return 0; }
                    if (exectimes.ContainsKey(a))
                    {
                        if (b == 0) return exectimes[a];
                        return Math.Max(exectimes[a], getexec(b) + 1);
                    }
                    else
                    { return 0; }// coroutine is executed 1 frame after added to queue
                }

                var skilldata = UnitSkillDatum.FromUnitId(prefab);
                foreach (var skill in new long[]
                {
                        skilldata.MainSkill1, skilldata.MainSkill2, skilldata.MainSkill3,skilldata.MainSkill4,skilldata.MainSkill5,
                        skilldata.MainSkill6, skilldata.MainSkill7, skilldata.MainSkill8,skilldata.MainSkill9,skilldata.MainSkill10,
                        skilldata.MainSkillEvolution1, skilldata.MainSkillEvolution2
                }.Where(s => s > 0))
                {
                    var data = SkillDatum.FromSkillId(skill);
                    var a = data.Actions; var b = data.DependActions;
                    dep = Enumerable.Range(0, 7).Where(i => a[i] > 0).ToDictionary(i => a[i], i => b[i]);
                    foreach (var act in a)
                        if (act > 0) exectime[act] = getexec(act);
                    actions[skill] = a.Where(a => a > 0).ToArray();
                }

        }
    }
}
