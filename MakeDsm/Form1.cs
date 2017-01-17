using MakeDsm.LinearDependencies;
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

        event EventHandler ModularityVM_Changed;
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
                this._modularityVM = null;
                
            }
        }

        ModularityMatrixVM _modularityVM;
        ModularityMatrixVM ModularityMatrix_VM
        {
            get { return this._modularityVM; }
            set
            {
                this._modularityVM = value;
                this.gvModularity.DataSource = this._modularityVM?.ModularityMatrix;
                this.ModularityVM_Changed?.Invoke(this,EventArgs.Empty);
                
            }
        }


        public Form1()
        {
            InitializeComponent();
            this.txbPath.Text = this.Path;
            this._DSM_VM = null;


            this.ModularityVM_Changed += Form1_ModularityVM_Changed;
            this.SetGridViewsStyle();

            this.dsm.DataBindingComplete += (s,e) => this.FillRowHeader(this.dsm, DSM_VM.COL_NAME);
            this.gvModularity.DataBindingComplete += (s,e) => this.FillRowHeader(this.gvModularity, ModularityMatrixVM.COL_METHOD_NAME);


        }

        private async void Form1_ModularityVM_Changed(object sender, EventArgs e)
        {
            var vm = this.ModularityMatrix_VM;
            if (vm != null)
            {
                Tuple<LinearRowDependencyLocator, LinearColumnDependencyLocator> locator = null;
                 await Task.Run(()=> locator = LinearDependencyLocator<object>.Factory(vm));
                locator.ToString();

                LinearRowDependencyLocator rowLocator = locator.Item1;
                LinearColumnDependencyLocator colLocator = locator.Item2;
                var rows = rowLocator.LinearDependedRows.ToDictionary(p => p.Key[ModularityMatrixVM.COL_METHOD_NAME].ToString(),p => p.Value.Select(r => r[ModularityMatrixVM.COL_METHOD_NAME].ToString()));
                var strRows = rows.Select(p => p.Key.ToString() + "\n" + String.Join("\n\t,", p.Value));

                var cols = colLocator.LinearDependedRows.ToDictionary(p => p.Key.ColumnName, p => p.Value.Select(c => c.ColumnName));
                var strCols = cols.Select(p => p.Key.ToString() + "\n" + String.Join("\n\t,", p.Value));

                var msg1 = String.Join("\n\n", strRows);
                var msg2 = String.Join("\n\n", strCols);

                System.Diagnostics.Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@ Rows:\n"+msg1);
                System.Diagnostics.Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@ Cols:\n" + msg2);
                MessageBox.Show(msg1);
                MessageBox.Show(msg2);

            }
        }

        private void SetGridViewsStyle()
        {
            
            var gvs = new List<DataGridView> {this.dsm, this.gvModularity };
            foreach (var gv in gvs)
            {
                gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                gv.ColumnHeadersHeight = 50;
                gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;

                gv.CellFormatting += new DataGridViewCellFormattingEventHandler(this.gv_CellFormatting);
                gv.CellPainting += new DataGridViewCellPaintingEventHandler(this.gv_CellPainting);
                gv.DataBindingComplete += this.gv_DataBindingComplete;

              
                gv.SelectionChanged += Gv_SelectionChanged;

            }

          
        }

        private void gv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var gv = sender as DataGridView;
            if (gv == null)
                return;
            foreach (DataGridViewColumn col in gv.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
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
                    this._DSM_VM = null;
                    
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
            
        }
        private void gvModularity_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            this.gvModularity.Columns[ModularityMatrixVM.COL_METHOD_NAME].Visible = false;
            this.gvModularity.Columns[ModularityMatrixVM.COL_SORT_VALUE].Visible = false;
            
        }

        private void FillRowHeader(DataGridView gv, string rowHeaderColName)
        {
            
            for (int i = 0; i < gv.Rows.Count; i++)
            {
                var header = gv.Rows[i].Cells[rowHeaderColName].Value;
                gv.Rows[i].HeaderCell.Value = header;
            }
        }

        private void gv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var gv = sender as DataGridView;
            if (gv == null)
                return;
            if (e.ColumnIndex < 0 ||  e.RowIndex < 0)
                return;

            var objVal = gv.Rows[e.RowIndex].Cells[e.ColumnIndex].Value ?? "";
            int val;
            int.TryParse(objVal.ToString(), out val);
            if (val == 1)
            {
                e.CellStyle.BackColor = Color.LightGray;
            }
        }
        private void Gv_SelectionChanged(object sender, EventArgs e)
        {
            var highlightColor = Color.Orange;
            var defColor = Color.FromKnownColor(KnownColor.Control);
            var gv = sender as DataGridView;
            if (gv == null)
                return;

            gv.TopLeftHeaderCell.Value = "";
            foreach (var row in gv.Rows.OfType<DataGridViewRow>())
                row.HeaderCell.Style.BackColor = defColor;

            foreach (var col in gv.Columns.OfType<DataGridViewColumn>())
                col.HeaderCell.Style.BackColor = defColor;

            var selectedRow = gv.SelectedCells.OfType<DataGridViewCell>().Select(c => c.OwningRow).Distinct().FirstOrDefault();
            if (selectedRow != null)
            {
                
                var rowHeader = selectedRow.HeaderCell;
                rowHeader.Style.BackColor = highlightColor;
                rowHeader.Style.SelectionBackColor = highlightColor;

                var headerText = selectedRow.HeaderCell.Value + "\n\n";

                var markedCels = selectedRow.Cells.OfType<DataGridViewCell>()
                                    .Where(c => !String.IsNullOrWhiteSpace((c.Value ?? "").ToString())).ToList();
                var markedColumns = markedCels.Select(c => c.OwningColumn).Where(col=> col.Visible);
                foreach (var clm in markedColumns)
                {
                    clm.HeaderCell.Style.BackColor = highlightColor;
                    headerText += clm.HeaderText + "\n";

                }

                gv.TopLeftHeaderCell.Value = headerText;
            }

        }
        private void gv_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var gv = sender as DataGridView;
            if (gv == null)
                return;
            // check that we are in a header cell!
            if (e.RowIndex == -1 && e.ColumnIndex >= 0)
            {
                e.PaintBackground(e.ClipBounds, true);
                Rectangle rect = gv.GetColumnDisplayRectangle(e.ColumnIndex, true);
                Size titleSize = TextRenderer.MeasureText(e.Value.ToString(), e.CellStyle.Font);
                if (gv.ColumnHeadersHeight < titleSize.Width)
                {
                    gv.ColumnHeadersHeight = titleSize.Width;
                }

                e.Graphics.TranslateTransform(0, titleSize.Width);
                e.Graphics.RotateTransform(-90.0F);

                // This is the key line for bottom alignment - we adjust the PointF based on the 
                // ColumnHeadersHeight minus the current text width. ColumnHeadersHeight is the
                // maximum of all the columns since we paint cells twice - though this fact
                // may not be true in all usages!   
                e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y - (gv.ColumnHeadersHeight - titleSize.Width), rect.X));

                // The old line for comparison
                //e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y, rect.X));


                e.Graphics.RotateTransform(90.0F);
                e.Graphics.TranslateTransform(0, -titleSize.Width);
                e.Handled = true;
            }
        }

      
        private void dsm_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
            if (e.RowIndex < 0)
                return;
            
            string className = this.dsm.Rows[e.RowIndex].Cells[DSM_VM.COL_NAME].Value as string;
            if (String.IsNullOrWhiteSpace(className))
                return;

          
        }

        private void tcDisplays_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tcDisplays.SelectedTab == this.tpModularity)
            {
                this.ModularityMatrix_VM = this._dsm_vm.GetModularityMatrix();
            }
        }
    }
}
