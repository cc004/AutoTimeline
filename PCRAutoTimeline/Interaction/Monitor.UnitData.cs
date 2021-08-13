using Elements;
using PCRApi.Models.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using PCRAutoTimeline.Models;

namespace PCRAutoTimeline.Interaction
{
    public static partial class Monitor
    {
        private class UnitData
        {
            public ActionState state;
            public int skillid, prefab, frame, framesince;
            public float time, last = 0f;
            public long handle;
            public long[] curaction;
            public readonly Dictionary<long, long> exectime = new Dictionary<long, long>();
            public readonly Dictionary<long, long[]> actions = new Dictionary<long, long[]>();

            public event Action<int> ActionExec;

            //private static long bossaddr;

            public void Refresh(int frame, float time)
            {
                //if (bossaddr == 0) bossaddr = Autopcr.getBossAddr(401041501);

                NativeFunctions.ReadProcessMemory(Program.hwnd, handle + 0x18C, out int state);
                if (curaction != null)
                    foreach (var action in curaction.Where(x => exectime[x] == framesince))
                    {
                       // Console.WriteLine($"{prefab} action executed {action}@{frame} {Autopcr.getDef(bossaddr)}");
                        ActionExec?.Invoke((int)action);
                    }
                ActionState cur = (ActionState)state;
                if (cur != this.state)
                {
                    curaction = null;
                    if (cur == ActionState.SKILL || cur == ActionState.SKILL_1)
                    {
                        NativeFunctions.ReadProcessMemory(Program.hwnd, handle + 0x110, out skillid);
                        curaction = actions[skillid];
                        framesince = Autopcr.frameoff;
                    }
                    else skillid = cur == ActionState.ATK ? 1 : 0;

                    this.frame = frame;
                    this.time = time;
                    //Console.WriteLine($"{prefab} action changed state={cur}/{skillid}@{frame}");
                }
                else
                    ++framesince;
                this.state = cur;
                last = time;
            }

            //const float delta = 1 / 60f;

            public void Initialize()
            {
                (_, prefab) = Autopcr.TryGetIntInt(Program.hwnd, handle + 0x244);
                //prefab = 107101;
                var prefabdata = UnitPrefabData.Get(prefab).UnitActionControllerData;
                var exectimes = prefabdata.MainSkillEvolutionList.Concat(prefabdata.MainSkillList)
                    .Concat(prefabdata.UnionBurstList).Concat(prefabdata.UnionBurstEvolutionList)
                    .SelectMany(skill => skill.ActionParametersOnPrefab)
                    .Where(prefab => prefab.Visible)
                    .SelectMany(prefab => prefab.Details)
                    .Where(det => det.Visible && det.ActionId > 0)
                    .ToDictionary(ap => (long)ap.ActionId, ap => 1 + (int)Math.Ceiling(60 * ap.ExecTimeForPrefab.Select(ex => ex.Time).Max()));
                Dictionary<long, long> dep = null;
                long getexec(long a)
                {
                    if (exectime.TryGetValue(a, out var res)) return res;
                    var b = dep[a];
                    if (b == 0) return exectimes[a];
                    return Math.Max(exectimes[a], getexec(b) + 1); // coroutine is executed 1 frame after added to queue
                }

                var skilldata = UnitSkillDatum.FromUnitId(prefab);
                foreach (var skill in new long[]
                {
                    skilldata.MainSkill1, skilldata.MainSkill2, skilldata.MainSkill3,skilldata.MainSkill4,skilldata.MainSkill5,
                    skilldata.MainSkill6, skilldata.MainSkill7, skilldata.MainSkill8,skilldata.MainSkill9,skilldata.MainSkill10,
                    skilldata.MainSkillEvolution1, skilldata.MainSkillEvolution2, skilldata.UnionBurst, skilldata.UnionBurstEvolution
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
}
