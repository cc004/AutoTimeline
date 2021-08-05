using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using CodeStage.AntiCheat.ObscuredTypes;
using Neo.IronLua;
using System.Threading;
using PCRAutoTimeline.Interaction;

namespace PCRAutoTimeline
{
    class Program
    {
        public static long hwnd, addr;

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
            NativeFunctions.ReadProcessMemory(hwnd, addr - 0x44, data, 16, 0);
            return (BitConverter.ToInt32(data, 0), BitConverter.ToSingle(data, 8));
        }

        private static int TryGetProcess()
        {
            foreach (var proc in Process.GetProcesses())
            {
                if (proc.ProcessName == "LdVBoxHeadless") return proc.Id;
            }

            return 0;
        }
        static void Main(string[] args)
        {
            //Minitouch.connect("localhost", 1111);
            //Minitouch.setPos(1, 100, 100);
            //Minitouch.press(1);
            
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            NativeFunctions.timeBeginPeriod(1);

            using var lua = new Lua();
            var env = lua.CreateEnvironment();

            env.RegisterPackage("autopcr", typeof(Autopcr));
            env.RegisterPackage("minitouch", typeof(Minitouch));
            env.RegisterPackage("input", typeof(Input));
            env.RegisterPackage("async", typeof(Async));

            LuaChunk chunk;
            var file = args.Length > 0 ? args[0] : "timeline.lua";

            try
            {
                chunk = lua.CompileChunk(File.ReadAllText(file), "timeline.lua", new LuaCompileOptions());
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"你{file}呢？");
                throw;
            }
            catch (LuaParseException e)
            {
                Console.WriteLine($"lua写错了！滚去学编程 行{e.Line}, 列{e.Column}");
                throw;
            }

            Console.Write("pid>");
            var str = Console.ReadLine();
            var pid = string.IsNullOrEmpty(str) ? TryGetProcess() : int.Parse(str);
            //var pid = 11892;
            hwnd = NativeFunctions.OpenProcess(NativeFunctions.PROCESS_ALL_ACCESS, false, pid);

            Console.Write("当前世界（以秒为单位，别给我填100,1.00，要是超过了20s直接挂树吧）");
            var time = int.Parse(Console.ReadLine());

            var tuple =  AobscanHelper.Aobscan(hwnd, idcode, addr =>
            {
                var frame = TryGetInfo(hwnd, addr);
                Console.WriteLine(frame);
                if (frame.Item1 >= 0 && frame.Item1 < 1200 && frame.Item2 > time - 1 && frame.Item2 < time + 1)
                {
                    Console.WriteLine($"data found, frameCount = {frame.Item1}, limitTime = {frame.Item2}");
                    return true;
                }
                return false;
            });

            addr = tuple.Item1;

            Console.WriteLine($"addr = {addr:x}");

            Console.WriteLine();

            if (addr == -1)
            {
                Console.WriteLine("没找到数据！好好看看是不是输错进程pid了或者没进对战，进对战不要开倍速！");
                throw new Exception();
            }
            
            seed_addr = AobscanHelper.Aobscan(hwnd, seed_code, addr =>
            {
                Console.WriteLine($"seed found.");
                return true;
            }).Item1 - 0x90;

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

            Async.start(() =>
            {
                chunk.Run(env);
                return null;
            });

            exiting = true;
            Minitouch.exit();

            Console.ReadLine();

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
