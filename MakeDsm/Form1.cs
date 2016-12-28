using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MakeDsm
{
    public partial class Form1 : Form
    {
        string Path
        {
            get
            {
                return Properties.Settings.Default?.Path ?? "";
            }
            set
            {
                Properties.Settings.Default.Path = value;
                Properties.Settings.Default.Save();
                this.txbPath.Text = value;
            }
        }
        public Form1()
        {
            InitializeComponent();
            this.txbPath.Text = this.Path;
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog()
            {
                FileName = Path
            };

            if (ofd.ShowDialog() == DialogResult.OK)
                this.Path = ofd.FileName;
        }

        private void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (!File.Exists(this.Path))
            {
                MessageBox.Show($"File '{this.Path}' does not exist.");
                return;
            }

            try
            {
                var result = MakeDsmService.GetDependencies(this.Path);
                IReadOnlyDictionary<string, List<string>> dependenciesByModule = result.DependencyDictionary;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured:\n{ex.Message}");
            }
            

        }
    }
}
