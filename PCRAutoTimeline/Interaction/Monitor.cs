using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRAutoTimeline.Interaction
{
    public static class Monitor
    {
        private class Unit
        {
            public long unit_handle;
            public int skillid;
            public int frame;
            public int Lframe;
            public string state;
            public bool is_self_buffed = true;
            public int self_buff_frame;
            public int self_buff_id;
            public long unit_id;

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
                Async.Await();
            }
        }

        private static void UpdateBuffCoroutine(string name)//开始调用这个函数后的第一个buff开始计时更新buff
        {
            var unit = units[name];
            int state_ret;
            int remain_buff_frame = unit.self_buff_frame;
            while (true) 
            {
                if (unit.is_self_buffed == true)
                {
                    if (remain_buff_frame > 0)
                    {
                        remain_buff_frame -= 1;
                        Autopcr.waitOneLFrame();
                    }
                    else
                    {
                        unit.is_self_buffed = false;
                    }
                }
                if (unit.skillid == unit.self_buff_id)
                { 
                    state_ret = waitSkillLFrame(name, unit.self_buff_id, UnitAutoData.getSkillExFrame(unit.unit_id, unit.self_buff_id));
                    if (state_ret == 0)//如果BUFF正常释放了
                    {
                        unit.is_self_buffed = true;
                        remain_buff_frame = unit.self_buff_frame;
                    }
                }
                Async.Await();
            }
        
        }

        public static void updateSelfBuff(string name,long unit_id)
        {
            units[name].unit_id = unit_id;
            var buff_id = UnitAutoData.getSelfBuffId(unit_id);
            if (buff_id != -1)
            {
                units[name].self_buff_id = buff_id;
                units[name].self_buff_frame = (int)Math.Ceiling(60 * UnitAutoData.getSelfBuffTime(unit_id)) - 4;//多给几帧容错
                units[name].is_self_buffed = false;
                Async.Start(() => UpdateBuffCoroutine(name));
            }
            else 
            {
                Console.WriteLine($"{name}角色，id为{unit_id}  没有查询到自buff");
                return;
            }


        }
        

        public static void add(string name, long unit_handle)
        {
            var unit = new Unit { unit_handle = unit_handle };
            
            units.Add(name, unit);
            Async.Start(() => Coroutine(unit));
        }

        public static string getActionState(string name)
        {
            return units[name].state;
        }

        public static bool getIsSelfBuffed(string name)
        {
            return units[name].is_self_buffed;
        }

        public static int getSkillId(string name)
        {
            return units[name].skillid;
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
