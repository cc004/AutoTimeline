using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PCRAutoTimeline.Interaction
{
    public class Input
    {
        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public static bool keyPressed(string vkey) => keyPressed(vkey[0]);
        public static bool keyPressed(int vkey)
        {
            return (GetAsyncKeyState(vkey) & 0x8000) == 0x8000;
        }
    }
}
