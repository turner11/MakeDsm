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
    class RowDependencyHighlighter: DependencyHighlighter<DataRow, DataGridViewRow>
    {
        protected override Color HighlightColor { get { return Color.LightSalmon; } }
        public RowDependencyHighlighter(DataGridView gv, LinearRowDependencyLocator dependencyLocator):base(gv, dependencyLocator)
        {

        }

        protected override DataGridViewRow GetView(DataRow row)
        {
            var gvRow = this.GridView.Rows.Cast<DataGridViewRow>()
                                    .FirstOrDefault(r => (((DataRowView)r.DataBoundItem).Row) == row);
            return gvRow;
        }

        protected override void PaintItem(DataGridViewRow row, Color color)
        {
            row.DefaultCellStyle = row.DefaultCellStyle  ?? new DataGridViewCellStyle(this.GridView.DefaultCellStyle);
            row.DefaultCellStyle.BackColor = color;
        }

        protected override DataRow GetItemFromSelectedCell(DataGridViewCell cell)
        {
            var gvRow = cell.OwningRow;
            var row = (gvRow.DataBoundItem as DataRowView)?.Row;
            return row;

            
        }
    }
}
