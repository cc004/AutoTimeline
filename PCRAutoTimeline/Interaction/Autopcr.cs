using CodeStage.AntiCheat.ObscuredTypes;
using Neo.IronLua;
using PCRAutoTimeline.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCRAutoTimeline
{
    public static class Autopcr
    {
        private static readonly Dictionary<int, NativeFunctions.POINT> mousepos = new();

        public static UnitCtrl getUnit(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle, out UnitCtrl unit);
            return unit;
        }
        public static void calibrate(string i)
        {
            Console.Write($"Mouse for pos {i}:");
            Console.ReadLine();
            NativeFunctions.GetCursorPos(out var pos);
            if (!mousepos.ContainsKey(i.GetHashCode())) mousepos.Add(i.GetHashCode(), pos);
            else mousepos[i.GetHashCode()] = pos;
            Console.WriteLine(pos);
        }
        public static void press(string id)
        {
            PressAt(mousepos[id.GetHashCode()]);
        }
        public static void framePress(string id)
        {
            framePress(id.GetHashCode());
        }

        public static void calibrate(int i)
        {
            Console.Write($"Mouse for pos #{i}:");
            Console.ReadLine();
            NativeFunctions.GetCursorPos(out var pos);
            if (!mousepos.ContainsKey(i)) mousepos.Add(i, pos);
            else mousepos[i] = pos;
            Console.WriteLine(pos);
        }

        public static void press(int id)
        {
            PressAt(mousepos[id]);
        }

        internal static int frameoff = 0;
        internal static float timeoff = 0;

        public static void setOffset(int frame, float time)
        {
            frameoff = frame;
            timeoff = time;
        }

        public static void waitOneFrame()
        {
            var frame = getFrame();
            _waitFrame(frame + 1);
        }

        public static void framePress(int id)
        {
            var point = mousepos[id];
            NativeFunctions.SetCursorPos(point.X, point.Y);
            NativeFunctions.mouse_event(NativeFunctions.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            waitOneFrame();
            NativeFunctions.mouse_event(NativeFunctions.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        private static bool UnitEvaluator(int unitid, int rarity, int promotion, long addr)
        {
            var t = TryGetIntInt(Program.hwnd, addr + 0x10);
            if (t.Item1 == rarity && t.Item2 == promotion)
            {
                var tp = getTp(addr - 0x244);
                var hp = getHp(addr - 0x244);
                var maxHp = getMaxHp(addr - 0x244);
                if (tp == 0.0 && maxHp == hp)
                {
                    Console.WriteLine($"unit found @{addr - 0x244:x} unitid = {unitid}, tp = {tp}, hp = {hp}/{maxHp}");
                    return true;
                }
            }
            return false;
        }

        private static bool BossEvaluator(int unitid, long addr)
        {
            var t = TryGetIntInt(Program.hwnd, addr + 0x10);
            var t2 = TryGetIntInt(Program.hwnd, addr);
            if (t.Item1 == 1 && t.Item2 == 1 && t2.Item2 >= 200000 && t2.Item2 <= 399999)
            {
                var tp = getTp(addr - 0x244);
                var hp = getHp(addr - 0x244);
                var maxHp = getMaxHp(addr - 0x244);
                if (tp == 0.0)
                {
                    Console.WriteLine($"unit found @{addr - 0x244:x} unitid = {unitid}, tp = {tp}, hp = {hp}/{maxHp}");
                    return true;
                }
            }
            return false;
        }

        public static long getUnitAddr(int unitid, int rarity, int promotion)
        {
            var b = BitConverter.GetBytes(unitid);
            var tuple = AobscanHelper.Aobscan(Program.hwnd, b.Concat(b).ToArray(),
                addr => UnitEvaluator(unitid, rarity, promotion, addr));
            return tuple.Item1 != -1 ? tuple.Item1 - 0x244 : -1;
        }


        public static long getBossAddr(int unitid)
        {
            var b = BitConverter.GetBytes(unitid);
            var tuple2 = AobscanHelper.Aobscan(Program.hwnd, b.ToArray(),
                addr => BossEvaluator(unitid, addr));
            return tuple2.Item1 != -1 ? tuple2.Item1 - 0x244 : -1;
        }


        public static float getTp(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x4E0, out ObscuredFloat tp);
            return tp;
        }

        public static long getHp(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x390, out ObscuredLong hp);
            return hp;
        }

        public static long getMaxHp(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x3A4, out ObscuredLong hp);
            return hp;
        }

        public static int getLevel(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x384, out ObscuredInt level);
            return level;
        }

        public static int getDef(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x3D0, out ObscuredInt def);
            return def;
        }

        public static int getMagicDef(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x3DC, out ObscuredInt def);
            return def;
        }

        public static int getPhysicalCritical(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x400, out ObscuredInt crit);
            return crit;
        }

        public static int getMagicCritical(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x40C, out ObscuredInt crit);
            return crit;
        }

        public static float getCrit(long unitHandle, long targetHandle, bool isMagic)
        {
            return (isMagic ? getMagicCritical(unitHandle) : getPhysicalCritical(unitHandle)) * 0.05f * 0.01f *
                getLevel(unitHandle) / getLevel(targetHandle);
        }

        public static string getActionState(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x18C, out int state);
            return ((ActionState)state).ToString();
        }

        public static float getCastTimer(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x194, out ObscuredFloat castTimer);
            return castTimer;
        }

        public static uint[] predRandom(int count)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, Program.seed_addr, out UnityRandom.state);
            var result = new uint[count];
            for (int i = 0; i < count; ++i) result[i] = UnityRandom.Random();
            return result;
        }

        public static float nextCrit()
        {
            return nextNCrit(1)[0];
        }

        public static float[] nextNCrit(int n)
        {
            var preds = predRandom(3 * n);
            return Enumerable.Range(0, n).Select(i => preds[2 * i + n + 1] % 1000 / 1000f).ToArray();
        }

        public static float[] nextCrits(LuaTable table)
        {
            var ns = table.ArrayList.Select(obj => (int)obj).ToArray();
            var preds = predRandom(ns.Last() + 1);
            return ns.Select(i => preds[i] % 1000 / 1000f).ToArray();
        }

        public static int critNum(long unitHandle, long targetHandle, bool isMagic, LuaTable table)
        {
            var crit = getCrit(unitHandle, targetHandle, isMagic);
            return nextCrits(table).Count(f => f < crit);
        }

        public static int getFrame()
        {
            return Program.TryGetInfo(Program.hwnd, Program.addr).Item1;
        }

        public static float getTime()
        {
            return Program.TryGetInfo(Program.hwnd, Program.addr).Item2;
        }

        private static void _waitFrame(int frame)
        {
            WaitFor(inf => inf.Item1 >= frame, inf => inf.Item1);
        }

        public static void waitFrame(int frame)
        {
            _waitFrame(frame - frameoff);
        }

        public static void waitTill(Func<bool> cond, int frameMax, int count)
        {
            int i = 0, start = getFrame();
            while (getFrame() < frameMax && i < count || start == getFrame())
                if (cond()) ++i; else i = 0;
        }

        public static void waitTillCrit(long unit, long target, bool isMagic, int frameMax)
        {
            waitTill(() =>
            {
                var crit = nextCrit() - getCrit(unit, target, isMagic);
                Console.WriteLine($"now crit = {crit}");
                return crit < 0;
            }, frameMax, 5);
        }

        public static void waitTillNCrit(long unit, long target, bool isMagic, int frameMax, int m, int n)
        {
            waitTill(() =>
            {
                var critrate = getCrit(unit, target, isMagic);
                var crit = nextNCrit(n).Count(crit => crit - critrate < 0);
                Console.WriteLine($"now crit = {crit}");
                return crit >= m;
            }, frameMax, 5);
        }

        public static void waitTillCrits(long unit, long target, bool isMagic, int frameMax, int m, LuaTable table)
        {
            waitTill(() =>
            {
                var critrate = getCrit(unit, target, isMagic);
                var crit = nextCrits(table).Count(crit => crit - critrate < 0);
                Console.WriteLine($"now crit = {crit}");
                return crit >= m;
            }, frameMax, 5);
        }

        public static void waitTime(float time)
        {
            WaitFor(inf => inf.Item2 <= time - timeoff, inf => inf.Item2);
        }

        internal static (int, int) TryGetIntInt(long hwnd, long addr)
        {
            var data = new byte[16];
            NativeFunctions.ReadProcessMemory(hwnd, addr, data, 16, 0);
            return (BitConverter.ToInt32(data, 0), BitConverter.ToInt32(data, 4));
        }

        private static void PressAt(NativeFunctions.POINT point)
        {
            NativeFunctions.SetCursorPos(point.X, point.Y);
            NativeFunctions.mouse_event(NativeFunctions.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            NativeFunctions.mouse_event(NativeFunctions.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }

        public static void bossub(int framecount)
        {
            frameoff -= framecount;
        }

        internal static void WaitFor(Func<(int, float), bool> check, Func<(int, float), float> changing)
        {
            (int, float) frame;
            float lastf = float.NaN;
            var last = -1;
            float lastff;
            do
            {
                lastff = lastf;
                frame = Program.TryGetInfo(Program.hwnd, Program.addr);
                if (frame.Item1 != last)
                {
                    Console.Write(
                        $"\rframeCount = {frame.Item1}, limitTime = {frame.Item2}                  ");
                    last = frame.Item1;
                }
                lastf = changing(frame);
                Thread.Sleep(1);
            } while (!check(frame) || !(changing(frame) != lastff && !float.IsNaN(lastff)));
            Console.WriteLine();
        }
        internal static void WaitFor(Func<(int, float), bool> check)
        {
            (int, float) frame;
            var last = -1;
            do
            {
                frame = Program.TryGetInfo(Program.hwnd, Program.addr);
                if (frame.Item1 != last)
                {
                    Console.Write(
                        $"\rframeCount = {frame.Item1}, limitTime = {frame.Item2}                  ");
                    last = frame.Item1;
                }
                Thread.Sleep(1);
            } while (!check(frame));
            Console.WriteLine();
        }


    }
}
