using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace basic_dll_injector
{
    public partial class MainWindow : Form
    {
        Process selectedProcess = null;
        string selectedFilePath = "";
        public MainWindow()
        {
            InitializeComponent();
            Text = $"Basic DLL Injector ({(Environment.Is64BitProcess ? "x64" : "x32")})";

            if (!Environment.Is64BitProcess)
            {
                MessageBox.Show("You're running the 32-bit version of 'basic dll injector'. You won't be able to choose 64-bit processes!", "Basic DLL Injector");
            }
        }

        private void cuiButton3_Click(object sender, EventArgs e)
        {
            var pp = new ProcessPicker();
            if (pp.ShowDialog() == DialogResult.OK)
            {
                if (pp.SelectedProcess == null)
                {
                    return;
                }

                selectedProcess = pp.SelectedProcess;
                cuiLabel1.Content = selectedProcess.ProcessName + ".exe";
            }
        }

        private void cuiButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog() {Filter= "DLL Files|*.dll" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                selectedFilePath = ofd.FileName;
                cuiLabel2.Content = ofd.SafeFileName;
            }
        }

        private void cuiButton1_Click(object sender, EventArgs e)
        {
            Injector.InjectDll(selectedFilePath, selectedProcess);
        }

        private void cuiButton1_Resize(object sender, EventArgs e)
        {
            cuiButton1.Refresh();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (selectedProcess == null || selectedProcess.HasExited || selectedFilePath == "" || !File.Exists(selectedFilePath))
            {
                cuiButton1.NormalBackground = Color.Gray;
                cuiButton1.Enabled = false;
            }
            else
            {
                cuiButton1.NormalBackground = Color.FromArgb(96, 106, 255);
                cuiButton1.Enabled = true;
            }
        }
    }
}
