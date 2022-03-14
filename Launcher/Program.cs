using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Launcher
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var info = new ProcessStartInfo
            {
                FileName = "python38\\python.exe",
                Arguments = string.Join(" ", args),
                UseShellExecute = false
            };
            Process.Start(info)?.WaitForExit();
            Thread.Sleep(100000);
        }
    }
}
