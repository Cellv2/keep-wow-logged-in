using System.Diagnostics;
using Tesseract;

namespace KeepWowLoggedIn
{
    public partial class Form1 : Form
    {
        private Helpers.Watcher? watcher = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Constants.ImageConstants.InstructionsImage;
            watcher = new Helpers.Watcher(pictureBox1, textBox1);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var engine = new TesseractEngine(@"./tessdata", "eng_best", EngineMode.Default);
            var img = Pix.LoadFromMemory(Utils.ImagingUtils.ImageToByteArray(pictureBox1.Image));
            var page = engine.Process(img);
            textBox1.Text = page.GetText();
            page.Dispose();
            img.Dispose();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Utils.ImagingUtils.SaveAndReturnTmpImageFromProcessId(selectedProcessId);
        }

        private int selectedProcessId = 0;
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (int.TryParse((comboBox1.SelectedItem.ToString() ?? "0").Split(" - ")[1], out selectedProcessId))
            {
                if (watcher == null)
                {
                    watcher = new Helpers.Watcher(pictureBox1, textBox1);
                }

                watcher.UpdatePictureImage(selectedProcessId);
            }
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

        // TODO: button to make the process window the active window? This way it must 
        // TODO: hotkey to force break/quit the application?
        private void btnBeginWatching_Click(object sender, EventArgs e)
        {
            if (selectedProcessId == 0)
            {
                MessageBox.Show("Please select a process to watch from the dropdown");
                return;
            }

            if (watcher == null)
            {
                watcher = new Helpers.Watcher(pictureBox1, textBox1);
            }

            watcher.StartWatchingProcess(selectedProcessId);
        }

        private void btnStopWatching_Click(object sender, EventArgs e)
        {
            if (watcher != null)
            {
                watcher.stopWatching();
            }
            else
            {
                MessageBox.Show("No process is being watched!\nPlease select a process from the dropdown");
            }
        }
    }
}
