using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesseract;

namespace KeepWowLoggedIn.Helpers
{
    public class Watcher
    {
        private bool _isWatching = false;
        private PictureBox _pictureBox;
        private TextBox _textBox;
        private TesseractEngine _tesseract;
        public Watcher(PictureBox pictureBox, TextBox textBox)
        {
            _pictureBox = pictureBox;
            _textBox = textBox;
            _tesseract = new TesseractEngine(@"./tessdata", "eng_best", EngineMode.Default);
        }

        // TODO: move this to imaging class? Or form class??
        public void UpdatePictureImage(int processId)
        {
            _pictureBox.Image = Utils.ImagingUtils.SaveAndReturnTmpImageFromProcessId(processId);
        }

        public async void StartWatchingProcess(int processId)
        {
            _isWatching = true;
            _textBox.Text = "watching started";

            //for (int i = 0; i < 100; i++)
            while (_isWatching)
            {
                UpdatePictureImage(processId);
                //var img = Pix.LoadFromMemory(Utils.ImagingUtils.ImageToByteArray(_pictureBox.Image));
                var img = Pix.LoadFromMemory(Utils.ImagingUtils.ImageToByteArray(_pictureBox.Image));
                var page = _tesseract.Process(img);
                string ocrText = page.GetText();

                // should not need these any longer as the text is now saved into the ocrText variable
                page.Dispose();
                img.Dispose();

                //TODO: check the exact words being used when a connection is trying to be made - we don't want to press return again while connecting!
                // if we get any of the messages, we probably just want to put a 5 sec sleep on it then continue down with the other contains() checks
                // perhaps this is just the word 'cancel'? all messages that I have seen for a reconnect state seem to have that as the confirm message

                // sometimes battlenet seems to 'disconnect' - we try capture this first and send escape once to cancel out as sending enter opens up a browser tab with a help page
                if (ocrText.Contains("Help", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("WOW51900325", StringComparison.OrdinalIgnoreCase))
                {
                    _textBox.Text = "'you have been disconnected' message appeared, sending escape";
                    SendKeys.Send("{ESC}");
                    continue;
                }

                // TODO: add more checks here
                // TODO: check what message comes up when your net drops
                // is "cancel" too generic?
                if (ocrText.Contains("Cancel", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("Logging in", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("Position in queue", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("queue", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("Estimated time", StringComparison.OrdinalIgnoreCase))
                {
                    _textBox.Text = "appear to be logging in or in a queue, no actions taken";
                    await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
                    continue;
                }

                if (ocrText.Contains("Enter World", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("Delete Character", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("AddOns", StringComparison.OrdinalIgnoreCase) || ocrText.Contains("Shop", StringComparison.OrdinalIgnoreCase))
                {
                    _textBox.Text = "appear to be on character select screen, no actions taken";
                    await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
                    continue;
                }

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
                    Utils.CursorUtils.ClickReconnectButton(processId);
                    await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
                }

                await Task.Run(() => Task.Delay(TimeSpan.FromSeconds(5)));
            }
        }

        public void stopWatching()
        {
            _isWatching = false;
            _textBox.Text = "watching stopped";
        }
    }
}
