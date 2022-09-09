using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepWowLoggedIn.Utils
{
    // https://docs.microsoft.com/en-us/dotnet/desktop/winforms/how-to-simulate-mouse-and-keyboard-events-in-code?view=netframeworkdesktop-4.8#to-send-a-keystroke-to-a-different-application



    public class CursorUtils
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public static void CalcDisconnectButtonLocAndClick()
        {

        }

        // the reconnect button is about 10% down on the y axis from the very center of the screen
        public static void CalcReconnectButtonLocAndClick(int imgWidth, int imgHeight)
        {
            int reconnectX = imgWidth / 2;
            int reconnectY = (int)((imgHeight / 2) - (imgHeight * 0.1));
            SetCursorPos(reconnectX, reconnectY);
            mouse_event(MOUSEEVENTF_LEFTDOWN, reconnectX, reconnectY, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, reconnectX, reconnectY, 0, 0);
        }
    }
}
