using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PCRAutoTimeline
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static class Minitouch
    {
        private static TcpClient client;
        private static Thread thread;
        private static BinaryReader br;
        private static int maxcontact, maxx, maxy, maxpressure;

        public static int getMaxContact() => maxcontact;
        public static int getMaxX() => maxx;
        public static int getMaxY() => maxy;
        public static int getMaxPressure() => maxpressure;

        private static bool exiting = false;

        public static void exit()
        {
            exiting = true;
            client?.Dispose();
        }

        static Minitouch()
        {
            thread = new Thread(() =>
            {
                while (!exiting)
                {
                    try
                    {
                        write("h"); // heartbeat
                    }
                    catch
                    {

                    }

                    Thread.Sleep(1000);
                }
            });
            thread.Start();
        }

        public static void connect(string host, int port)
        {
            client = new TcpClient();
            client.NoDelay = true;
            client.Connect(host, port);
            br = new BinaryReader(client.GetStream());
            stream = client.GetStream();

            var res = read();
            res = read();
            maxcontact = int.Parse(res[0]);
            maxx = int.Parse(res[1]);
            maxy = int.Parse(res[2]);
            maxpressure = int.Parse(res[3]);

            Console.WriteLine($"minitouch connected @{host}:{port} ({maxx}x{maxy})");
        }

        private static NetworkStream stream;

        public static void write(string cmd)
        {
            lock (client)
            {
                stream.Write(Encoding.ASCII.GetBytes(cmd + "\n"));
            }
        }

        public static string[] read()
        {
            lock (client)
            {
                List<byte> res = new ();
                for (;;)
                {
                    var b = br.ReadByte();
                    if (b == '\n') return Encoding.ASCII.GetString(res.ToArray()).Split(' ').Skip(1).ToArray();
                    res.Add(b);
                }
            }
        }

        private static readonly Dictionary<int, (int, int)> pos = new ();

        public static void setPos(string id, int x, int y)
        {
            setPos(id.GetHashCode(), x, y);
        }

        public static void press(string id)
        {
            press(id.GetHashCode());
        }

        public static void framePress(string id)
        {
            framePress(id.GetHashCode());
        }

        public static void setPos(int id, int x, int y)
        {
            if (pos.ContainsKey(id)) pos[id] = (x, y);
            else pos.Add(id, (x, y));
        }

        public static void press(int id)
        {
            write($"d 0 {pos[id].Item1} {pos[id].Item2} 1\nc\nu 0\nc");
            stream.Flush();
        }

        public static void framePress(int id)
        {
            write($"d 0 {pos[id].Item1} {pos[id].Item2} 1\nc");
            stream.Flush();
            Autopcr.waitOneFrame();
            write("u 0\nc");
            stream.Flush();
        }
    }

}
