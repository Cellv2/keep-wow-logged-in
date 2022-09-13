using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeepWowLoggedIn.Utils
{
    public class ImagingUtils
    {

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public static Image? SaveAndReturnTmpImageFromProcessId(int processId)
        {
            // 0 is our default - if it is 0 then it hasn't been changed
            if (processId == 0)
            {
                // TODO: center this over the running form
                MessageBox.Show("Please select a process from the dropdown");
                return Constants.ImageConstants.InstructionsImage;
            };

            Process process = Process.GetProcessById(processId);
            if (process == null || !SetForegroundWindow(process.MainWindowHandle))
            {
                return Constants.ImageConstants.InstructionsImage;
            }

            if (SetForegroundWindow(process.MainWindowHandle))
            {
                // force window to top before any screenshots are taken
                SendKeys.Send(" ");

                RECT srcRect;
                if (!process.MainWindowHandle.Equals(IntPtr.Zero))
                {
                    if (GetWindowRect(process.MainWindowHandle, out srcRect))
                    {
                        int width = srcRect.Right - srcRect.Left;
                        int height = srcRect.Bottom - srcRect.Top;

                        Bitmap bmp = new Bitmap(width, height);
                        Graphics screenG = Graphics.FromImage(bmp);

                        try
                        {
                            screenG.CopyFromScreen(srcRect.Left, srcRect.Top,
                                    0, 0, new Size(width, height),
                                    CopyPixelOperation.SourceCopy);

                            using (FileStream fs = new FileStream(Path.GetTempFileName(),
                               FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None,
                               4096, FileOptions.RandomAccess | FileOptions.DeleteOnClose))
                            {
                                // temp file exists
                                bmp.Save(fs, ImageFormat.Jpeg);
                                return Image.FromStream(fs);
                            }

                            // tmp image should be deleted here due to FileOptions.DeleteOnClose

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                            return null;
                        }
                        finally
                        {
                            screenG.Dispose();
                            bmp.Dispose();
                        }
                    }
                }
            }

            return null;
        }
    }
}
