using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm.LinearDependencies
{
    public class LinearColumnDependencyLocator : LinearDependencyLocator<DataColumn>
    {
        private readonly DataTable _dt;
        private ReadOnlyCollection<DataRow> _rowsCache;
        private ReadOnlyCollection<DataRow> _rows
        {
            get
            {
                this._rowsCache = this._rowsCache ?? _dt.AsEnumerable().ToList().AsReadOnly();
                return this._rowsCache;
            }
        }

        public IReadOnlyCollection<DataColumn> Columns {get{ return this.Items; }}
        internal LinearColumnDependencyLocator(DataTable dt) : base(dt.Columns.OfType<DataColumn>().Where(c=> c.DataType == typeof(string)).ToList())
        {
            this._dt = dt;
            
        }


        
        protected override bool[] ToLogicalArrayInternal(DataColumn column)
        {
            if (column.ColumnName == ModularityMatrixVM.COL_METHOD_NAME || column.ColumnName == ModularityMatrixVM.COL_SORT_VALUE)
                return this._rows.Select(r => false).ToArray();

            bool[] logical = this._rows.Select(r => (r.Field<object>(column) ?? "0").ToString() == "1").ToArray();
           
            
            return logical;
        }
    }
}
