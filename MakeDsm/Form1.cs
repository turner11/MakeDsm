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
                var strPathes = Properties.Settings.Default?.Paths.Split(',')?.FirstOrDefault() ?? "";
                return strPathes;
            }
            set
            {
                var AllPathes = Properties.Settings.Default?.Paths.Split(',').ToList();
                AllPathes.Insert(0, value);
                AllPathes = AllPathes.Where(p=> !string.IsNullOrWhiteSpace(p)).Distinct().Take(5).ToList();

                Properties.Settings.Default.Paths = string.Join(",", AllPathes);
                Properties.Settings.Default.Save();
                this.cmbPath.Items.Clear();
                this.cmbPath.Items.AddRange(AllPathes.Cast<object>().ToArray());
                this.cmbPath.Text = value;
            }
        }

        DSM_VM _dsm_vm;
        DSM_VM _DSM_VM
        {
            get { return this._dsm_vm; }
            set
            {
                this._dsm_vm = value;
                this.dsm.DataSource = this._dsm_vm?.DSM;
                this.ModularityMatrix_VM = this._DSM_VM?.GetModularityMatrix();


            }
        }

        ModularityMatrixVM _modularityVM;
        private RowDependencyHighlighter _rowLinearDependencyHighLighter;
        private ColumnDependencyHighlighter _colLinearDependencyHighliter;

        bool isLastHighlighterCol;

        ModularityMatrixVM ModularityMatrix_VM
        {
            get { return this._modularityVM; }
            set
            {
                this._modularityVM = value;
                this.gvModularity.DataSource = this._modularityVM?.ModularityMatrix;
                this.ModularityVM_Changed?.Invoke(this, EventArgs.Empty);

            }
        }



        public Form1()
        {
            InitializeComponent();
            this.cmbPath.Text = this.Path;
            this._DSM_VM = null;


            this.ModularityVM_Changed += Form1_ModularityVM_Changed;
            this.SetGridViewsStyle();

            this.dsm.DataBindingComplete += (s, e) => this.FillRowHeader(this.dsm, DSM_VM.COL_NAME);
            this.gvModularity.DataBindingComplete += (s, e) => this.FillRowHeader(this.gvModularity, ModularityMatrixVM.COL_METHOD_NAME);
            this.pnlButtons.Invalidated += PnlButtons_Invalidated;
            this.lblIdx.Text = "";
            this.gvModularity.CellClick += GvModularity_CellClick; 
;

        }

      

        private void PnlButtons_Invalidated(object sender, InvalidateEventArgs e)
        {
            var btns = new List<Button>
            {
                this.btnDown,
                this.btnUp,
                this.btnRight,
                this.btnLeft
            };

            foreach (var btn in btns)
            {
                btn.InvokeIfRequired(() => btn.Refresh());
            }
        }



        private async void Form1_ModularityVM_Changed(object sender, EventArgs e)
        {
            var vm = this.ModularityMatrix_VM;
            if (vm != null)
            {
                try
                {
                    this.SetDependencyControlEnableState(false);

                    Tuple<LinearRowDependencyLocator, LinearColumnDependencyLocator> locator = null;
                    await Task.Run(() => locator = LinearDependencyLocator<object>.Factory(vm));

                    var rowLocator = locator.Item1;
                    var colLocator = locator.Item2;
                    this._rowLinearDependencyHighLighter = new RowDependencyHighlighter(this.gvModularity, rowLocator);
                    this._colLinearDependencyHighliter = new ColumnDependencyHighlighter(this.gvModularity, colLocator);
                    this.ResetHighlighters();


                    //var rows = rowLocator.LinearDependedRows.ToDictionary(p => p.Key[ModularityMatrixVM.COL_METHOD_NAME].ToString(), p => p.Value.Select(r => r[ModularityMatrixVM.COL_METHOD_NAME].ToString()));
                    //var strRows = rows.Select(p => p.Key.ToString() + "\n\t" + String.Join("\n\t,", p.Value));

                    //var cols = colLocator.LinearDependedRows.ToDictionary(p => p.Key.ColumnName, p => p.Value.Select(c => c.ColumnName));
                    //var strCols = cols.Select(p => p.Key.ToString() + "\n\t" + String.Join("\n\t,", p.Value));

                    //var msg1 = String.Join("\n\n", strRows);
                    //var msg2 = String.Join("\n\n", strCols);

                    //System.Diagnostics.Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@ Rows:\n" + msg1);
                    //System.Diagnostics.Debug.WriteLine("@@@@@@@@@@@@@@@@@@@@@@@@@@ Cols:\n" + msg2);
                    //MessageBox.Show(msg1);
                    //MessageBox.Show(msg2);
                    this.SetDependencyControlEnableState(true);
                }
                catch (Exception ex)
                {

                    this.SetDependencyControlEnableState(false);
                    MessageBox.Show("An error occurred while getting dependencies.\n" + ex.ToString());
                }

            }
            else
            {
                this.SetDependencyControlEnableState(false);
            }
        }

        private void SetDependencyControlEnableState(bool setEnable)
        {
            //var btns = new List<Button>
            //{
            //    this.btnDown,
            //    this.btnUp,
            //    this.btnRight,
            //    this.btnLeft
            //};

            //foreach (var btn in btns)
            //{
            //    btn.InvokeIfRequired(() => btn.Enabled = setEnable);
            //}
            this.pnlButtons.Visible = setEnable;
            this.pnlButtons.Refresh();
        }

        private void SetGridViewsStyle()
        {

            var gvs = new List<DataGridView> { this.dsm, this.gvModularity };
            foreach (var gv in gvs)
            {
                gv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
                gv.ColumnHeadersHeight = 50;
                gv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCellsExceptHeader;

                gv.CellFormatting += new DataGridViewCellFormattingEventHandler(this.gv_CellFormatting);
                gv.CellPainting += new DataGridViewCellPaintingEventHandler(this.gv_CellPainting);
                gv.DataBindingComplete += this.gv_DataBindingComplete;


                gv.SelectionChanged += Gv_SelectionChanged;
                gv.DoubleBuffered(true);
            }


        }

        private void gv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            var gv = sender as DataGridView;
            if (gv == null)
                return;


            foreach (DataGridViewColumn col in gv.Columns)
                col.SortMode = DataGridViewColumnSortMode.NotSortable;
            gv.AutoResizeRowHeadersWidth(DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders);
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
            var path = this.Path;
            if (!File.Exists(path))
            {
                MessageBox.Show($"File '{this.Path}' does not exist.");
                return;
            }

            try
            {
                this.Path = path; // this will set this as default...
                using (new CursorWait())
                {
                    this.lblIdx.Text = "";
                    this.btnAnalyze.Enabled = false;
                    this._DSM_VM = null;

                    IDenpendencies result = null;
                    await Task.Run(() => result = MakeDsmService.GetDependencies(this.Path));
                    IReadOnlyDictionary<string, List<string>> dependenciesByModule = result.DependencyDictionary;

                    var vm = new DSM_VM(result);
                    this._DSM_VM = vm;


                }

            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occured:\n{ex.Message}");
            }
            finally
            {
                this.btnAnalyze.Enabled = true;

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
            if (e.ColumnIndex < 0 || e.RowIndex < 0)
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
            //var highlightColor = Color.Orange;
            //var defColor = Color.FromKnownColor(KnownColor.Control);
            var gv = sender as DataGridView;
            if (gv == null)
                return;

           

            gv.TopLeftHeaderCell.Value = "";
            //foreach (var row in gv.Rows.OfType<DataGridViewRow>())
            //    row.HeaderCell.Style.BackColor = defColor;

            //foreach (var col in gv.Columns.OfType<DataGridViewColumn>())
            //    col.HeaderCell.Style.BackColor = defColor;

            var selectedRow = gv.SelectedCells.OfType<DataGridViewCell>().Select(c => c.OwningRow).Distinct().FirstOrDefault();
            if (selectedRow != null)
            {

                var rowHeader = selectedRow.HeaderCell;
                //rowHeader.Style.BackColor = highlightColor;
                //rowHeader.Style.SelectionBackColor = highlightColor;

                var headerText = selectedRow.HeaderCell.Value + "\n\n";

                var markedCels = selectedRow.Cells.OfType<DataGridViewCell>()
                                    .Where(c => !String.IsNullOrWhiteSpace((c.Value ?? "").ToString())).ToList();
                var markedColumns = markedCels.Select(c => c.OwningColumn).Where(col => col.Visible);
                foreach (var clm in markedColumns)
                {
                    //clm.HeaderCell.Style.BackColor = highlightColor;
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

                var maxHiegt = 200;
                var h = Math.Min(titleSize.Width, maxHiegt);

                if (gv.ColumnHeadersHeight < titleSize.Width)
                {
                    gv.ColumnHeadersHeight = titleSize.Width;
                    gv.ColumnHeadersHeight = h;
                }

                e.Graphics.TranslateTransform(0, titleSize.Width);
                e.Graphics.RotateTransform(-90.0F);

                // This is the key line for bottom alignment - we adjust the PointF based on the 
                // ColumnHeadersHeight minus the current text width. ColumnHeadersHeight is the
                // maximum of all the columns since we paint cells twice - though this fact
                // may not be true in all usages!   


                e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y - (gv.ColumnHeadersHeight - titleSize.Width), rect.X));
                // e.Graphics.DrawString(e.Value.ToString(), this.Font, Brushes.Black, new PointF(rect.Y - (h - titleSize.Width), rect.X));

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
                //this.ModularityMatrix_VM = this._dsm_vm?.GetModularityMatrix();
            }
        }
        private void SetIndexText<TModel, TView>(DependencyHighlighter<TModel, TView> dependencyHighLighter) where TModel : class
        {
            this.lblIdx.Text = $"{dependencyHighLighter.Idx +1} / {dependencyHighLighter.ItemCount}";
        } 

        private void btnDown_Click(object sender, EventArgs e)
        {
            this.ResetHighlighters();
            this._rowLinearDependencyHighLighter?.Next();
            this.SetIndexText(this._rowLinearDependencyHighLighter);
            this.pnlButtons.Refresh();
            this.isLastHighlighterCol = false;

        }



        private void btnUp_Click(object sender, EventArgs e)
        {
            this.ResetHighlighters();
            this._rowLinearDependencyHighLighter?.Previous();
            this.SetIndexText(this._rowLinearDependencyHighLighter);
            this.pnlButtons.Refresh();
            this.isLastHighlighterCol = false;


        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            this.ResetHighlighters();
            this._colLinearDependencyHighliter?.Next();
            this.SetIndexText(this._colLinearDependencyHighliter);
            this.isLastHighlighterCol = true;
            this.pnlButtons.Refresh();
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            this.ResetHighlighters();
            this._colLinearDependencyHighliter?.Previous();
            this.SetIndexText(this._colLinearDependencyHighliter);
            this.pnlButtons.Refresh();
            this.isLastHighlighterCol = true;
        }

        private void ResetHighlighters()
        {
            this._colLinearDependencyHighliter?.Reset();
            this._rowLinearDependencyHighLighter?.Reset();
            this.pnlButtons.Refresh();
         
        }
        
            
        private void GvModularity_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
            if (e.ColumnIndex <= this.gvModularity.ColumnCount 
                &&  e.RowIndex <= this.gvModularity.RowCount 
                && e.ColumnIndex >=0
                &&  e.RowIndex >= 0)
            {
                try
                {
                    //this.gvModularity.CellClick -= GvModularity_CellClick;

                    this.ResetHighlighters();
                    var cell = this.gvModularity[e.ColumnIndex, e.RowIndex];
                    if (this.isLastHighlighterCol)
                        this._colLinearDependencyHighliter?.HighlightDependencies(cell);
                    else
                        this._rowLinearDependencyHighLighter?.HighlightDependencies(cell);


                }
                finally
                {
                    //this.gvModularity.CellClick += GvModularity_CellClick;
                }


            }
        }

        private void cmbPath_SelectedValueChanged(object sender, EventArgs e)
        {
            var path = this.cmbPath.SelectedItem as string;
            if (String.IsNullOrWhiteSpace(path))
                return;

            if (this.Path != path)
            {
                this.Path = path;

            }
            //this.cmbPath.Text = path;
        }
    }
}
