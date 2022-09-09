using IronOcr;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace KeepWowLoggedIn
{
    public partial class Form1 : Form
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

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //pictureBox1.ImageLocation = @"test.png";
            pictureBox1.ImageLocation = @".\Img\instructions.png";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IronTesseract IronOcr = new IronTesseract();

            var plainText = IronOcr.Read(pictureBox1.ImageLocation);

            textBox1.Text = plainText.Text;
        }

        //TODO: on exit, clean up screenshots
        private void button2_Click(object sender, EventArgs e)
        {
            // TODO: maybe some kind of popup saying to select a process?
            // 0 is our default - if it is 0 then it hasn't been changed
            if (selectedProcessId == 0) return;

            Process proc = Process.GetProcessById(selectedProcessId);
            if (SetForegroundWindow(proc.MainWindowHandle))
            {
                RECT srcRect;
                if (!proc.MainWindowHandle.Equals(IntPtr.Zero))
                {
                    if (GetWindowRect(proc.MainWindowHandle, out srcRect))
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
                                pictureBox1.Image = Image.FromStream(fs);
                            }

                            // tmp image should be deleted here due to FileOptions.DeleteOnClose

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                        finally
                        {
                            screenG.Dispose();
                            bmp.Dispose();
                        }
                    }
                }
            }
        }

        private int selectedProcessId = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int.TryParse((comboBox1.SelectedItem.ToString() ?? "0").Split(" - ")[1], out selectedProcessId);
        }

        // TODO: when selecting dropdown item, update screenshot in tooling
        private void comboBox1_OnDropdownOpened(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            var processes = Process.GetProcesses();
            var filteredProcesses = processes.ToList().FindAll(p => p.MainWindowTitle == "World of Warcraft");
            foreach (var process in filteredProcesses)
            {
                comboBox1.Items.Add($"{process.ProcessName} - {process.Id}");
            }               
        }

        private void btnBeginWatching_Click(object sender, EventArgs e)
        {
            IronTesseract IronOcr = new IronTesseract();

            string ocrText = IronOcr.Read(pictureBox1.Image).Text;

            if (ocrText.Contains("Reconnect", StringComparison.OrdinalIgnoreCase))
            {

            }

            textBox1.Text = ocrText;
        }
    }
}
