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

        DSM_VM _dsm_vm;
        DSM_VM _DSM_VM
        {
            get { return this._dsm_vm; }
            set { this._dsm_vm = value;
                this.dsm.DataSource = this._dsm_vm?.DSM;
                
            }
        }

        public Form1()
        {
            InitializeComponent();
            this.txbPath.Text = this.Path;

            this.dsm.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            this.dsm.ColumnHeadersHeight = 50;
            this.dsm.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;
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

        private async void btnAnalyze_Click(object sender, EventArgs e)
        {
            if (!File.Exists(this.Path))
            {
                MessageBox.Show($"File '{this.Path}' does not exist.");
                return;
            }

            try
            {
                using (new CursorWait())
                {
                    IDenpendencies result = null;
                    await Task.Run(()=> result = MakeDsmService.GetDependencies(this.Path));
                    IReadOnlyDictionary<string, List<string>> dependenciesByModule = result.DependencyDictionary;

                    var vm = new DSM_VM(result);
                    this._DSM_VM = vm;
                    
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured:\n{ex.Message}");
            }


        }

        private void dsm_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.dsm.Columns[DSM_VM.COL_NAME].Visible = false;
            this.dsm.Columns[DSM_VM.COL_SORT].Visible = false;
            this.FillRowHeader();
        }

        private void FillRowHeader()
        {
            var count = Math.Min(this.dsm.Rows.Count, this.dsm.Columns.Count);
            for (int i = 0; i < count; i++)
            {
                
                this.dsm.Rows[i].HeaderCell.Value = this.dsm.Rows[i].Cells[DSM_VM.COL_NAME].Value;
            }
        }

        private void dsm_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0 ||  e.RowIndex < 0)
                return;

            var objVal = this.dsm.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ?? "";
            int val;
            int.TryParse(objVal.ToString(), out val);
            if (val == 1)
            {
                e.CellStyle.BackColor = Color.LightGray;
            }
        }

        private void dsm_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // check that we are in a header cell!
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = this.dsm.GetColumnDisplayRectangle(e.ColumnIndex, true);
                Size titleSize = TextRenderer.MeasureText(e.Value.ToString(), e.CellStyle.Font);
                if (this.dsm.ColumnHeadersHeight < titleSize.Width)
                {
                    this.dsm.ColumnHeadersHeight = titleSize.Width;
                }

                e.Graphics.TranslateTransform(0, titleSize.Width);
                e.Graphics.RotateTransform(-90.0F);

                // This is the key line for bottom alignment - we adjust the PointF based on the 
                // ColumnHeadersHeight minus the current text width. ColumnHeadersHeight is the
                // maximum of all the columns since we paint cells twice - though this fact
                // may not be true in all usages!   
                e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y - (dsm.ColumnHeadersHeight - titleSize.Width), rect.X));

                // The old line for comparison
                //e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y, rect.X));


                e.Graphics.RotateTransform(90.0F);
                e.Graphics.TranslateTransform(0, -titleSize.Width);
                e.Handled = true;
            }
        }

        private class SortByColumOrder : IComparer<DataGridViewRow>, System.Collections.IComparer
        {
            private readonly DataGridView gv;

            public SortByColumOrder(DataGridView gv)
            {
                this.gv = gv;
            }

            public int Compare(object x, object y)
            {
                return Compare(x as DataGridViewRow, y as DataGridViewRow);
            }

            public int Compare(DataGridViewRow x, DataGridViewRow y)
            {
                var nameX = x.Cells[DSM_VM.COL_NAME].Value.ToString();
                var nameY = y.Cells[DSM_VM.COL_NAME].Value.ToString();
                var idxX = this.gv.Columns[nameX].Index;
                var idxY = this.gv.Columns[nameY].Index;

                return idxX-idxY;
                
            }
        }
    }
}
