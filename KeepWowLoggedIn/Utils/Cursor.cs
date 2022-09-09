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
        // Activate an application window.
        [DllImport("USER32.DLL")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        public static async void ClickDisconnectedButton(int processId)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // Press return and then sleep for 5 seconds
        public static async void ClickReconnectButtonAsync(int processId)
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
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
