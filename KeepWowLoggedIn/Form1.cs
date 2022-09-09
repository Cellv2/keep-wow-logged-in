using IronOcr;
using System.Diagnostics;

namespace KeepWowLoggedIn
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pictureBox1.ImageLocation = @".\Img\instructions.png";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IronTesseract IronOcr = new IronTesseract();

            var plainText = IronOcr.Read(pictureBox1.ImageLocation);

            textBox1.Text = plainText.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            pictureBox1.Image = Utils.ImagingUtils.SaveAndReturnTmpImageFromProcessId(selectedProcessId);
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

        // TODO: button to make the process window the active window? This way it must 
        // TODO: hotkey to force break/quit the application?
        private void btnBeginWatching_Click(object sender, EventArgs e)
        {
            IronTesseract IronOcr = new IronTesseract();

            string ocrText = IronOcr.Read(pictureBox1.Image).Text;

            //TODO: check the exact words being used when a connection is trying to be made - we don't want to press return again while connecting!
            // if we get any of the messages, we probably just want to put a 5 sec sleep on it then continue down with the other contains() checks
            // perhaps this is just the word 'cancel'? all messages that I have seen for a reconnect state seem to have that as the confirm message

            if (!ocrText.Contains("Disconnect", StringComparison.OrdinalIgnoreCase) && !ocrText.Contains("Reconnect", StringComparison.OrdinalIgnoreCase))
            {
                textBox1.Text = "reconnect not found, no actions taken";
                return;
            }

            if (ocrText.Contains("Disconnect", StringComparison.OrdinalIgnoreCase))
            {
                textBox1.Text = "disconnect found";
                Utils.CursorUtils.ClickDisconnectedButton(selectedProcessId);
            }

            if (ocrText.Contains("Reconnect", StringComparison.OrdinalIgnoreCase))
            {
                textBox1.Text = "reconnect found";
                Utils.CursorUtils.ClickReconnectButtonAsync(selectedProcessId);
            }

            //Thread.Sleep(TimeSpan.FromSeconds(120));
            //textBox1.Text = ocrText;
        }
    }
}
