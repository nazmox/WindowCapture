using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Timer = System.Windows.Forms.Timer;

namespace WindowCaptureOwn
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hdcBlt, uint nFlags);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetWindowRect(IntPtr hWnd, ref RECT rect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private Timer captureTimer;
        private IntPtr windowHandle;

        public Form1()
        {
            InitializeComponent();

            // Find the window you want to capture (by title or class name)
            windowHandle = FindWindow(null, "World of Warcraft");

            // Set up a timer to periodically capture the window
            captureTimer = new Timer();
            captureTimer.Interval = 10;  // Capture every 1 second (adjust as needed)
            captureTimer.Tick += CaptureTimer_Tick;
            captureTimer.Start();
        }

        private void CaptureTimer_Tick(object sender, EventArgs e)
        {
            // Capture the window and display the captured content
            CaptureWindow();
        }

        private void CaptureWindow()
        {
            if (windowHandle != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetWindowRect(windowHandle, ref rect);

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                Bitmap bmp = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(bmp))
                {
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(windowHandle, hdc, 0);
                    g.ReleaseHdc(hdc);
                }

                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Image = bmp;
            }
            else
            {
                MessageBox.Show("Window not found.");
            }
        }

    }

}