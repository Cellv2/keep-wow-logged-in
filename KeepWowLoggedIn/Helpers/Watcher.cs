using IronOcr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepWowLoggedIn.Helpers
{
    public class Watcher
    {
        private bool _isWatching = false;
        private PictureBox _pictureBox;
        private TextBox _textBox;
        public Watcher(PictureBox pictureBox, TextBox textBox)
        {
            _pictureBox = pictureBox;
            _textBox = textBox;
        }

        public void UpdatePictureImage(int processId)
        {
            _pictureBox.Image = Utils.ImagingUtils.SaveAndReturnTmpImageFromProcessId(processId);
        }

        public async void StartWatchingProcess(int processId)
        {
            _isWatching = true;
            int x = 10;
            //while (_isWatching)
            for (int i = 0; i < 100; i++)
            {
                IronTesseract IronOcr = new IronTesseract();


                UpdatePictureImage(processId);
                string ocrText = IronOcr.Read(_pictureBox.Image).Text;

                //TODO: check the exact words being used when a connection is trying to be made - we don't want to press return again while connecting!
                // if we get any of the messages, we probably just want to put a 5 sec sleep on it then continue down with the other contains() checks
                // perhaps this is just the word 'cancel'? all messages that I have seen for a reconnect state seem to have that as the confirm message

                if (!ocrText.Contains("Disconnect", StringComparison.OrdinalIgnoreCase) && !ocrText.Contains("Reconnect", StringComparison.OrdinalIgnoreCase))
                {
                    _textBox.Text = "reconnect not found, no actions taken";
                    await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
                    continue;
                }

                if (ocrText.Contains("Disconnect", StringComparison.OrdinalIgnoreCase))
                {
                    _textBox.Text = "disconnect found";
                    Utils.CursorUtils.ClickDisconnectedButton(processId);
                    await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
                }

                if (ocrText.Contains("Reconnect", StringComparison.OrdinalIgnoreCase))
                {
                    _textBox.Text = "reconnect found";
                    Utils.CursorUtils.ClickReconnectButtonAsync(processId);
                    await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
                }

                await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
            }
        }

        public void stopWatching()
        {
            _isWatching = false;
        }
    }
}
