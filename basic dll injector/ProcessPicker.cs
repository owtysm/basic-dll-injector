using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basic_dll_injector
{
    public partial class ProcessPicker : Form
    {
        public Process SelectedProcess;
        public ProcessPicker()
        {
            InitializeComponent();
            foreach (Process process in Process.GetProcesses().Where(p => !string.IsNullOrEmpty(p.MainWindowTitle)).Distinct())
            {
                flowLayoutPanel1.Controls.Add(new ProcessHolderControl(process, this));
            }
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
