using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Diagnostics;
using System.Runtime.InteropServices;

namespace KeepWowLoggedIn.Utils
{


    public class CursorUtils
    {
        // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/how-to-simulate-mouse-and-keyboard-events-in-code?view=netframeworkdesktop-4.8#to-send-a-keystroke-to-a-different-application
        // Get a handle to an application window.
        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        private static extern IntPtr FindWindow(string lpClassName,
            string lpWindowName);

        // Activate an application window.
        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);


        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }


        [DllImport("USER32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [DllImport("USER32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void ClickDisconnectedButton(int processId)
        {
            var process = Process.GetProcessById(processId);
            if (process == null) return;

            var handle = process.MainWindowHandle;
            if (handle == IntPtr.Zero)
            {
                MessageBox.Show("The selected process is not running");
                return;
            };

            try
            {
                SetForegroundWindow(handle);
                SendKeys.Send("{ENTER}");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Press return and then sleep for 5 seconds
        public static void ClickReconnectButton(int processId)
        {
            var process = Process.GetProcessById(processId);
            if (process == null) return;

            var handle = process.MainWindowHandle;
            if (handle == IntPtr.Zero)
            {
                MessageBox.Show("The selected process is not running");
                return;
            };

            try
            {
                SetForegroundWindow(handle);
                SendKeys.Send("{ENTER}");
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
