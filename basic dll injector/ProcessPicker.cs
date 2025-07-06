using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basic_dll_injector
{
    public partial class ProcessPicker : Form
    {
        public Process SelectedProcess;
        public bool ChooseByWindow = false;
        public ProcessPicker(bool byWindow)
        {
            InitializeComponent();
            ChooseByWindow = byWindow;

            var processes = Process.GetProcesses();
            processes = byWindow
                ? processes.Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)).OrderBy(p => p.MainWindowTitle).ToArray()
                : processes.OrderBy(p => p.ProcessName).ToArray();

            const int itemHeight = 38;
            int currentY = 0;
            int currentIndex = 1;
            var form = MainWindow.ActiveForm;
            string textBefore = form.Text;

            panel1.SuspendLayout();

            foreach (var process in processes)
            {
                try
                {
                    var handle = process.SafeHandle;
                    if (!handle.IsClosed && !handle.IsInvalid && !process.HasExited)
                    {
                        panel1.Controls.Add(new ProcessHolderControl(process, this)
                        {
                            Top = currentY,
                            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                            Width = panel1.Width
                        });

                        currentY += itemHeight;
                    }
                }
                catch { }

                if (currentIndex % 10 == 0)
                    form.Text = $"{currentIndex}/{processes.Length}";

                currentIndex++;
            }

            panel1.ResumeLayout();
            form.Text = textBefore;
        }

        private void cuiButton3_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void cuiButton1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
