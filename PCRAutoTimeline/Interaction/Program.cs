using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;

namespace PCRAutoTimeline.Interaction
{
    public class Program
    {
        public static long hwnd, addr;
        public static IntPtr main_handle;
        public static bool is_init_main_handle = false;

        public static long seed_addr;


        private static readonly byte[] idcode =
        {
            0x3c, 0, 0, 0,
            0x89, 0x88, 0x88, 0x3C
        };

        private static readonly byte[] seed_code = Encoding.UTF8.GetBytes(
            "_GlobalWindTime\0_WindQuality\0\0\0\0_Wind\0\0\0\0\0\0\0\0\0\0\0_Shininess\0\0\0\0\0\0");

        public static (int, float) TryGetInfo(long hwnd, long addr)
        {
            var data = new byte[16];
            NativeFunctions.ReadProcessMemory(hwnd, addr - 0x48, data, 16, 0);
            return (BitConverter.ToInt32(data, 0), BitConverter.ToSingle(data, 8));
        }

        private static int TryGetProcess()
        {
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "NemuHeadless") return proc.Id;
                if (proc.ProcessName == "LdVBoxHeadless") return proc.Id;
                if (proc.ProcessName == "LdBoxHeadless") return proc.Id;
            }

            return 0;
        }

        internal static float timeOffsetByTotal;

        public static void Main()
        {
            //Minitouch.connect("localhost", 1111);
            //Minitouch.setPos(1, 100, 100);
            //Minitouch.press(1);
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            NativeFunctions.timeBeginPeriod(1);
            
            Console.Write("内核pid，如果是单开，直接回车即可，程序自动搜索>");
            var str = Console.ReadLine();
            var pid = !int.TryParse(str, out var val) ? TryGetProcess() : val;
            //var pid = 11892;
            hwnd = NativeFunctions.OpenProcess(NativeFunctions.PROCESS_ALL_ACCESS, false, pid);
            Console.WriteLine("载入全角色数据");
            UnitAutoData.Init(); //载入不怎么占用时间，直接在主程序载入了

            Console.Write("总刀长度（以秒为单位，不填默认90）");
            var s = str == "debug" ? "" : Console.ReadLine();
            var total = string.IsNullOrEmpty(s) ? 90 : int.Parse(s);
            timeOffsetByTotal = 90 - total;
            Autopcr.setOffset(0, 0);

            Console.Write("当前世界（以秒为单位，别给我填100,1.00，要是超过了20s直接挂树吧）");
            var time = str == "debug" ? 0 : int.Parse(Console.ReadLine());
            
            var tuple =  AobscanHelper.Aobscan(hwnd, idcode, addr =>
            {
                var frame = TryGetInfo(hwnd, addr);
                Console.WriteLine(frame);
                if (frame.Item1 >= 0 && frame.Item1 < 1200 && frame.Item2 > time - 1 && frame.Item2 < time + 1)
                {
                    Console.WriteLine($"data found, rFrame = {frame.Item1}, lFrame = {(90 - frame.Item2) * 60}, lTime = {frame.Item2}");
                    return true;
                }
                return false;
            });


            addr = tuple.Item1;

            Console.WriteLine($"addr = {addr:x}");

            Console.WriteLine();

            if (addr == -1 && str != "debug")
            {
                Console.WriteLine("没找到数据！好好看看是不是输错进程pid了或者没进对战，进对战不要开倍速！");
                throw new Exception();
            }
            
            seed_addr = AobscanHelper.Aobscan(hwnd, seed_code, addr =>
            {
                Console.WriteLine($"seed found.");
                return true;
            }).Item1 - 0x90;

            Async.StartCurrent();
            /*
            UnityRandom.State state0;
            NativeFunctions.ReadProcessMemory(Program.hwnd, Program.seed_addr, out state0);
            UnityRandom.State state;
            int last = 0;
            while (true)
            {
                NativeFunctions.ReadProcessMemory(Program.hwnd, Program.seed_addr, out state);
                int i = 0;
                List<float> rands;
                UnityRandom.state = state0;
                while (state.x != UnityRandom.state.x)
                {
                    ++i;
                    UnityRandom.Random();
                }
                if (i > last) Console.WriteLine($"rand times={i}");
                last = i;
            }
            
            */
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Console.WriteLine($"程序出错了！赶紧手打或者挂树吧，以下是详细信息：\n{e.ExceptionObject}");
            Console.WriteLine("按enter退出");
            Console.ReadLine();
        }

        public static bool exiting;
    }
}
