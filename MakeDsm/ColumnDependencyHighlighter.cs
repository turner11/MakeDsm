using MakeDsm.LinearDependencies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace MakeDsm
{
    class ColumnDependencyHighlighter : DependencyHighlighter<DataColumn,DataGridViewColumn>
    {
        protected override Color HighlightColor{ get { return Color.Orange; } }
        public ColumnDependencyHighlighter(DataGridView gv, LinearColumnDependencyLocator dependencyLocator):base(gv, dependencyLocator)
        {
            this.PrePaintingItems += ColumnDependencyHighlighter_PrePaintingItems;
        }

        private void ColumnDependencyHighlighter_PrePaintingItems(object sender, EventArgs e)
        {
            //resseting desfult styles (white box: need to reset the row styles highliter...)
            //var gvRows = this.GridView.Rows.Cast<DataGridViewRow>().ToList();
            //foreach (var row in gvRows)
            //    row.DefaultCellStyle = null;

            //var ds = this.GridView.DataSource;
            //this.GridView.DataSource = null;
            //this.GridView.DataSource = ds;

            var idx = this.GridView.FirstDisplayedScrollingRowIndex;
            DataTable dt = this.GridView.DataSource as DataTable;
            if (dt != null)
            {

                dt.AcceptChanges();
                this.GridView.FirstDisplayedScrollingRowIndex = idx;
            }
        }

        protected override DataGridViewColumn GetView(DataColumn dataColumn)
        {

            var gvCol = this.GridView.Columns.Cast<DataGridViewColumn>()
                                     .FirstOrDefault(c => c.Name == dataColumn.ColumnName);

            return gvCol;

        }

        protected override void PaintItem(DataGridViewColumn column, Color color)
        {
            column.DefaultCellStyle.BackColor = color;
        }
    }
}
