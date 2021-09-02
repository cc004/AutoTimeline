using CodeStage.AntiCheat.ObscuredTypes;
using PCRAutoTimeline.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;


namespace PCRAutoTimeline.Interaction
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

        public static void waitOneLFrame()
        {
            var frame = getLFrame();
            waitLFrame(frame + 1);
        }

        public static void framePress(int id)
        {
            var point = mousepos[id];
            NativeFunctions.SetCursorPos(point.X, point.Y);
            NativeFunctions.mouse_event(NativeFunctions.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
            waitOneFrame();
            NativeFunctions.mouse_event(NativeFunctions.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
        }


        public static void sleep(int time)
        {
            for (int i = 0; i < time; ++i) Async.Await();
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

        private static bool BossAutoEvaluator(long addr)
        {
            var t = TryGetIntInt(Program.hwnd, addr + 0x10);
            var t2 = TryGetIntInt(Program.hwnd, addr);
            if (t.Item1 == 1 && t.Item2 == 1 && t2.Item2 >= 200000 && t2.Item2 <= 399999)
            {
                var tp = getTp(addr - 0x244);
                var hp = getHp(addr - 0x244);
                var maxHp = getMaxHp(addr - 0x244);
                if (tp == 0.0 && maxHp<=15000000 && (maxHp%10000)==0)
                {
                    Console.WriteLine($"unit found @{addr - 0x244:x} , tp = {tp}, hp = {hp}/{maxHp}");
                    return true;
                }
            }
            return false;
        }

        private static bool UnitAutoEvaluator( long addr)
        {
            var t = TryGetIntInt(Program.hwnd, addr + 0x10);
            if ((t.Item1 >= 1 && t.Item1 <= 6) && (t.Item2 >= 1 && t.Item2 <= 50)) //rank到50总够用了
            {
                var tp = getTp(addr - 0x244);
                var hp = getHp(addr - 0x244);
                var maxHp = getMaxHp(addr - 0x244);
                if (tp == 0.0 && maxHp == hp && maxHp >0 && maxHp<100000)
                {
                    Console.WriteLine($"unit found @{addr - 0x244:x} star = {t.Item1},rank = {t.Item2} , tp = {tp}, hp = {hp}/{maxHp}");
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

        public static long getUnitAddrEasy(int unitid)
        {
            var b = BitConverter.GetBytes(unitid);
            var tuple = AobscanHelper.Aobscan(Program.hwnd, b.Concat(b).ToArray(),
                addr => UnitAutoEvaluator(addr));
            return tuple.Item1 != -1 ? tuple.Item1 - 0x244 : -1;
        }




        public static long getBossAddr(int unitid)
        {
            var b = BitConverter.GetBytes(unitid);
            var tuple2 = AobscanHelper.Aobscan(Program.hwnd, b.ToArray(),
                addr => BossEvaluator(unitid, addr));
            return tuple2.Item1 != -1 ? tuple2.Item1 - 0x244 : -1;
        }

        public static void multipress(string id, int dur)
        {
            for (int i = 0; i < dur; ++i)
            {
                press(id);
                waitOneFrame();
            }
        }

        private static Dictionary<int, string> Tuple2dic((int, long, long, string) tuple)
        {
            var dic_tuple = new Dictionary<int, string>();
            dic_tuple[0] = tuple.Item1.ToString();
            dic_tuple[1] = tuple.Item2.ToString();
            dic_tuple[2] = tuple.Item3.ToString();
            dic_tuple[3] = tuple.Item4.ToString();
            return dic_tuple;
        }

        public static Dictionary<int, string>[] autoGetBossAddr()
        {
            var res_list = new List<Dictionary<int, string>>();
            var b_low = 401000000;
            var b_high = 410000000;
            var search_list = AobscanHelper.Compscan(Program.hwnd, b_low,b_high,
                addr => BossAutoEvaluator(addr),AobscanHelper.MemmemBossComp);
            var boss_name = new string("");
            int order = 0;
            
            foreach (var temp_tuple in search_list) 
            {
                if (temp_tuple.Item1 != -1 && ((boss_name = UnitAutoData.getBossName(temp_tuple.Item2)) != "未找到该Boss"||((boss_name = UnitAutoData.getBossPartsName(temp_tuple.Item2)) != "未知部位")))
                {
                    
                    res_list.Add(Tuple2dic((order, temp_tuple.Item1 - 0x244, temp_tuple.Item2, boss_name)));
                    order += 1;
                }
            }
            return res_list.ToArray();
        }

        public static Dictionary<int, string>[] autoGetUnitAddr()
        {
            var res_list = new List<Dictionary<int, string>>();
            var b_low = 100101;
            var b_high = 190801;
            var search_list = AobscanHelper.Compscan(Program.hwnd, b_low, b_high,
                addr => UnitAutoEvaluator(addr),AobscanHelper.MemmemUnitComp);
            var unit_name = new string("");
            int order = 0;

            foreach (var temp_tuple in search_list)
            {
                if (temp_tuple.Item1 != -1 && (unit_name=UnitAutoData.getUnitName(temp_tuple.Item2) )!= "未知角色")
                { 
                    res_list.Add(Tuple2dic((order,temp_tuple.Item1 - 0x244, temp_tuple.Item2,unit_name)));
                    order += 1;
                }
            }
            return res_list.ToArray();
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
        
        public static int getSkillId(long unitHandle)
        {
            NativeFunctions.ReadProcessMemory(Program.hwnd, unitHandle + 0x110, out int skillid);
            return skillid;
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

        public static float[] nextCrits(int[] table)
        {
            var ns = table.Select(obj => (int)obj).ToArray();
            var preds = predRandom(ns.Last() + 1);
            return ns.Select(i => preds[i] % 1000 / 1000f).ToArray();
        }

        public static int critNum(long unitHandle, long targetHandle, bool isMagic, int[] table)
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

        public static int getLFrame()
        {
            return (int)(90 - Program.TryGetInfo(Program.hwnd, Program.addr).Item2)*60;
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

        public static void waitTillCrits(long unit, long target, bool isMagic, int frameMax, int m, int[] table)
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

        public static void waitLFrame(int frame)
        {
            WaitForNoPrint(inf => inf.Item2 <= (5400-frame)/60f - timeoff, inf => inf.Item2);
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
                        $"\rrFrame = {frame.Item1}, lFrame = {(90 - frame.Item2) * 60}, lTime = {frame.Item2}                                 ");
                    last = frame.Item1;
                }
                lastf = changing(frame);
                Async.Await();
            } while (!check(frame) || !(changing(frame) != lastff && !float.IsNaN(lastff)));
            Console.WriteLine();
        }

        internal static void WaitForNoPrint(Func<(int, float), bool> check, Func<(int, float), float> changing)
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
                    last = frame.Item1;
                }
                lastf = changing(frame);
                Async.Await();
            } while (!check(frame) || !(changing(frame) != lastff && !float.IsNaN(lastff)));
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
                Async.Await();
            } while (!check(frame));
            Console.WriteLine();
        }

        private static int TryGetDnplayerProcess()
        {
            foreach (var proc in System.Diagnostics.Process.GetProcesses())
            {
                if (proc.ProcessName == "dnplayer") return proc.Id;
            }

            return 0;
        }
        public static void switchToGameInit()//用后台的handle不行，要用dnpayer的
        {
            Console.Write("dnplayer.exe的pid，如果是单开，直接回车即可，程序自动搜索>");
            var str = Console.ReadLine();
            var pid = string.IsNullOrEmpty(str) ? TryGetDnplayerProcess() : int.Parse(str);
            Program.main_handle = System.Diagnostics.Process.GetProcessById(pid).MainWindowHandle;
            Program.is_init_main_handle = true;
        }
        public static void switchToGame()//用后台的handle不行，要用dnpayer的,所以要先初始化
        {
            if (Program.is_init_main_handle) 
            { NativeFunctions.SetForegroundWindow(Program.main_handle); }
            else 
            { Console.WriteLine("需要先初始化dnplayer.exe，调用autopcr.SwitchToGameInit()"); }
            
        }

    }
}
