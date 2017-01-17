using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm.LinearDependencies
{
    public class LinearRowDependencyLocator : LinearDependencyLocator<DataRow>
    {
        public ReadOnlyCollection<DataRow> Rows { get { return this.Items; } }
        

        //NOTE: this has some assumptions! (sorting is negative, no other int columns that might be 1...)
        Func<DataRow, bool[]> rowToLogicalArray = (r) => r.ItemArray.Select(obj => (obj ?? "0").ToString() != "1").ToArray();

        internal LinearRowDependencyLocator(IList<DataRow> rows):base(rows)
        {
        
           
        }

        protected override bool[] ToLogicalArrayInternal(DataRow row)
        {
            return rowToLogicalArray(row);
        }
    }
}
