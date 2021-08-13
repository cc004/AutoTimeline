using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRAutoTimeline.Interaction
{
    public static partial class Monitor
    {
        private class Unit
        {
            public long unit_handle;
            public UnitData unit_data;
            public int skillid;
            public int frame;
            public int Lframe;
            public string state;
        }

        private static Dictionary<string, Unit> units = new();

        private static void Coroutine(Unit unit)
        {
            while (true)
            {
                unit.state = Autopcr.getActionState(unit.unit_handle);
                if (unit.state == "SKILL" || unit.state == "SKILL_1" || unit.state == "ATK")
                {
                    var t = Autopcr.getSkillId(unit.unit_handle);
                    if (unit.skillid != t)
                    {
                        unit.skillid = t;
                        unit.frame = Autopcr.getFrame();
                        unit.Lframe = Autopcr.getLFrame();
                    }
                }
                else unit.skillid = 0;
                var (frame, time) = Program.TryGetInfo(Program.hwnd, Program.addr);
                Async.await();
            }
        }


        public static void add(string name, long unit_handle)
        {
            var unit = new Unit { unit_handle = unit_handle };
            unit.unit_data = new UnitData { handle = unit.unit_handle };
            unit.unit_data.Initialize();
            units.Add(name, unit);
            Async.start(() => Coroutine(unit));
        }

        public static string getActionState(string name)
        {
            return units[name].state;
        }

        public static int getSkillId(string name)
        {
            return units[name].skillid;
        }

        public static long getSkillExFrame(string name,long skillid)
        {
            if (skillid == 0)
            {
                return 0;
            }
            else if (skillid == 1)
            {
                return 0;
            }

            var skill_action_frame = new List<long>();
            var unit = units[name];
            unit.unit_data.actions.TryGetValue(skillid, out var skill_action_list);
            foreach (long action_id in skill_action_list)
            {
                unit.unit_data.exectime.TryGetValue(action_id, out var temp_frame);
                skill_action_frame.Add(temp_frame);


            }
            var max_frame = skill_action_frame.Max();

            return max_frame;
        }

        public static int getSkillFrame(string name)
        {
            return units[name].frame;
        }

        public static int getSkillLFrame(string name)
        {
            return units[name].Lframe;
        }

        public static void waitSkill(string name, int skill, int deltaFrame)
        {
            var unit = units[name];
            if (unit.skillid != skill)
                Autopcr.WaitFor(_ => unit.skillid == skill, i => i.Item1);
            Autopcr.WaitFor(i => i.Item1 >= unit.frame + deltaFrame - Autopcr.frameoff, i => i.Item1);
        }

        public static int waitSkillLFrame(string name, int skill, int deltaFrame)
        {
            var unit = units[name];
            var realdeltaFrame = deltaFrame;
            var SignFirstIn = false; //标记是否技能已经相同过，0表示没有，1表示有过
            var returnSign = 0;//技能等待返回状态，标记技能是否被打断，技能等待是否正常，0表示没有，1表示有，2表示异常返回（目前只有循环次数过多造成的异常）
            var maxWait = 500; //最多等待500个逻辑帧
            do
            {
                if (unit.skillid != skill && SignFirstIn == false)
                {
                    Autopcr.waitOneLFrame();
                    maxWait -= 1;
                }
                else if (unit.skillid != skill && SignFirstIn == true)
                {
                    returnSign = 1; //进入技能后，但没等到技能生效技能状态就改变了，说明技能被打断了
                    break;
                }
                else if (unit.skillid == skill && SignFirstIn == false) //首次进入目标等待技能
                {
                    SignFirstIn = true;//标记已经进入过该技能
                    realdeltaFrame -= Autopcr.getLFrame() - unit.Lframe - Autopcr.frameoff + 1;
                    maxWait -= 1;
                    Autopcr.waitOneLFrame();
                }
                else
                {
                    maxWait -= 1;
                    realdeltaFrame -= 1;
                    Autopcr.waitOneLFrame();
                }

            } while (realdeltaFrame > 0 && maxWait>0);
            if (maxWait == 0)
            {
                returnSign = 2;
            }
            return returnSign;
        }
    }
}
