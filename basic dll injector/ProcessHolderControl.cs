using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace basic_dll_injector
{
    public partial class ProcessHolderControl : UserControl
    {
        ProcessPicker pp;
        Process process;
        public ProcessHolderControl(Process _process, ProcessPicker _pp)
        {
            InitializeComponent();

            pp = _pp;
            process = _process;

            cuiLabel1.Content = process.MainWindowTitle;
            pictureBox1.Image = GetProcessIconBitmap(process);
        }

        private Image GetProcessIconBitmap(Process process)
        {
            try
            {
                string exePath = process.MainModule?.FileName;
                if (!string.IsNullOrEmpty(exePath) && File.Exists(exePath))
                {
                    using (Icon icon = Icon.ExtractAssociatedIcon(exePath))
                    {
                        return icon?.ToBitmap();
                    }
                }
            }
            catch
            {
                // dont bother - it's probably the 32 bit version trying to access a 64 bit process's icon
                Height = 0;
                Visible = false;
                process = null;
                pp = null;
                cuiLabel1.Dispose();
                pictureBox1.Dispose();
            }
            return null;
        }

        private void cuiLabel1_Click(object sender, System.EventArgs e)
        {
            pp.SelectedProcess = process;
            pp.cuiLabel1.Content = $"Selected: {process.ProcessName}.exe";
        }
    }
}
