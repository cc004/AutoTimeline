using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCRAutoTimeline
{
    public static partial class Monitor
    {
        
        private enum ActionState
        {
            IDLE = 0, ATK, SKILL_1, SKILL,
            WALK, DAMAGE, SUMMON, DIE, GAME_START,
            LOSE
        }

        private static Dictionary<long, UnitData> units = new Dictionary<long, UnitData>();

        public static string getActionState(long unitHandle)
        {
            return units[unitHandle].state.ToString();
        }

        public static int getSkillId(long unitHandle)
        {
            return units[unitHandle].skillid;
        }

        public static void waitAction(long unitHandle, int actionid)
        {
            bool action = false;
            void setaction(int id) { if (id == actionid) action = true; }
            units[unitHandle].ActionExec += setaction;
            Autopcr.WaitFor(_ => action, pair => pair.Item2);
            units[unitHandle].ActionExec -= setaction;
        }

        public static void add(long handle)
        {
            var data = new UnitData
            {
                handle = handle
            };
            data.Initialize();
            units.Add(handle, data);
        } 
        public static void start()
        {
            new Thread(() =>
            {
                var last = -1;
                var sw = new Stopwatch();
                sw.Start();
                while (!Program.exiting)
                {
                    var (frame, time) = Program.TryGetInfo(Program.hwnd, Program.addr);
                    if (frame != last)
                    {
                        foreach (var pair in units) pair.Value.Refresh(frame, -time);
                        last = frame;
                    }
                    Thread.Sleep(1);
                }
            }).Start();
        }
    }
}
