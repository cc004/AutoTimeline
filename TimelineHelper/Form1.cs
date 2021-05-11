using PCRAutoTimeline;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimelineHelper
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private long addr = -1;
        private int hwnd;

        private static readonly byte[] idcode =
        {
            0x3c, 0, 0, 0,
            0x89, 0x88, 0x88, 0x3C
        };

        public static (int, float) TryGetInfo(long hwnd, long addr)
        {
            var data = new byte[16];
            NativeFunctions.ReadProcessMemory(hwnd, addr - 0x44, data, 16, 0);
            return (BitConverter.ToInt32(data, 0), BitConverter.ToSingle(data, 8));
        }

        private void button1_Click(object sender, EventArgs e)
        {

            var pid = int.Parse(textBox1.Text);
            //var pid = 11892;
            hwnd = NativeFunctions.OpenProcess(NativeFunctions.PROCESS_ALL_ACCESS, false, pid);
            int i = 0;

            var tuple = AobscanHelper.Aobscan(hwnd, idcode, addr =>
            {
                var frame = TryGetInfo(hwnd, addr);
                if (frame.Item1 >= 0 && frame.Item1 < 1000 && frame.Item2 > 80 && frame.Item2 < 100)
                {
                    label3.Text = ($"data found, frameCount = {frame.Item1}, limitTime = {frame.Item2}");
                    return true;
                }
                return false;
            }, callback: s =>
            {
                label3.Text = s;
                if (++i % 100 == 0)
                    Refresh();
            });

            addr = tuple.Item1;

            label3.Text = ($"addr = {addr:x}");

            if (addr == -1)
            {
                label3.Text = ("aobscan failed.");
                return;
            }

            button1.Visible = false;
            textBox1.Visible = false;
            radioButton1.Visible = false;
            radioButton2.Visible = false;
            label3.Visible = false;
            label1.Visible = false;
        }

        private int frame, last;

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (addr == -1) return;
            var g = e.Graphics;
            var w = e.ClipRectangle.Width;
            var x = (int)(w * (frame % 120 / 120f));
           // g.DrawLine(Pens.Red, new Point(x, 0), new Point(x, 200));
            g.FillRectangle(Brushes.Yellow, new Rectangle(x - 1, 0, 2, 200));
            this.Text = "block=" + (frame / 120).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (addr == -1) return;
            var (frame, time) = TryGetInfo(hwnd, addr);
            this.frame = radioButton1.Checked ? frame : (int)(60 * (90 - time));
            if (last != this.frame)
                Refresh();
            last = this.frame;
        }
    }
}
