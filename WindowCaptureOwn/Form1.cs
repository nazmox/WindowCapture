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
            pictureBox1.SizeMode= PictureBoxSizeMode.StretchImage;
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
            CaptureWindowAndDisplayWithCursor();
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

        private void CaptureWindowAndDisplayWithCursor()
        {
            if (windowHandle != IntPtr.Zero)
            {
                RECT rect = new RECT();
                GetWindowRect(windowHandle, ref rect);

                int width = rect.Right - rect.Left;
                int height = rect.Bottom - rect.Top;

                // Capture the game window
                Bitmap gameImage = new Bitmap(width, height);

                using (Graphics g = Graphics.FromImage(gameImage))
                {
                    IntPtr hdc = g.GetHdc();
                    PrintWindow(windowHandle, hdc, 0);
                    g.ReleaseHdc(hdc);
                }

                // Capture the cursor image
                CURSORINFO pci;
                pci.cbSize = Marshal.SizeOf(typeof(CURSORINFO));
                GetCursorInfo(out pci);

                if (pci.flags == 1) // Check if the cursor is visible (1 indicates visible)
                {
                    // Get the cursor handle
                    IntPtr cursorHandle = CopyIcon(pci.hCursor);

                    using (Graphics g = Graphics.FromImage(gameImage))
                    {
                        // Create a Cursor object using the handle
                        Cursor cursor = new Cursor(cursorHandle);

                        // Draw the cursor image onto the game image
                        Rectangle cursorRect = new Rectangle(pci.ptScreenPos.x - rect.Left, pci.ptScreenPos.y - rect.Top, cursor.Size.Width, cursor.Size.Height);
                        cursor.Draw(g, cursorRect);
                    }

                    // Destroy the cursor handle
                    DestroyIcon(cursorHandle);
                }

                // Display the game image (with or without cursor) in the PictureBox
                pictureBox1.Image = gameImage;
            }
            else
            {
                MessageBox.Show("Window not found.");
            }
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CURSORINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hCursor;
            public POINT ptScreenPos;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CopyIcon(IntPtr hIcon);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool DestroyIcon(IntPtr hIcon);


    }

}