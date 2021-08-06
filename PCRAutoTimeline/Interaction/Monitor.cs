using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCRAutoTimeline.Interaction
{
    public class Monitor
    {
        private class Unit
        {
            public long unit_handle;
            public int skillid;
            public int frame;
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
                    }
                }
                else unit.skillid = 0;

                Async.await();
            }
        }

        public static void add(string name, long unit_handle)
        {
            var unit = new Unit { unit_handle = unit_handle };
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

        public static int getSkillFrame(string name)
        {
            return units[name].frame;
        }

        public static void waitSkill(string name, int skill, int deltaFrame)
        {
            var unit = units[name];
            if (unit.skillid != skill)
                Autopcr.WaitFor(_ => unit.skillid == skill, i => i.Item1);
            Autopcr.WaitFor(i => i.Item1 >= unit.frame + deltaFrame - Autopcr.frameoff, i => i.Item1);
        }
    }
}
