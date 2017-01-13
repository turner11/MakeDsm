using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace MakeDsm
{
    internal class ModularityMatrixVM
    {
        public const string COL_METHOD_NAME ="METHOD NAME";
        public const string COL_SORT_VALUE ="SORT VALUE";
        private readonly ClassessWithMethods _classedWithMethods;
        public DataTable ModularityMatrix { get; }

        public ModularityMatrixVM(ClassessWithMethods classessWithMethods)
        {
            this._classedWithMethods = classessWithMethods;
            var dtMethodsByClass = this.GenerateModularityMatrix();
            this.ModularityMatrix = this.MakeBlockDiagonalTable(dtMethodsByClass);
           
        }

        private DataTable MakeBlockDiagonalTable(DataTable dtMethodsByClass)
        {
            //var rows = dtMethodsByClass.Rows.OfType<DataRow>().ToList();
            //var cols = dtMethodsByClass.Columns.OfType<DataColumn>().ToList();

            //var colGroups = cols.Select(c=> new { ID = cols.IndexOf(c), Values = new List<string> {c.ColumnName } });
            //var rowGroups = rows.Select(r=> new { ID = -rows.IndexOf(r), Values = new List<string> {r[COL_METHOD_NAME].ToString() } });

            //foreach (var row in rows)
            //{
            //    foreach (var col in cols)
            //    {
            //        var value = (row[col] ?? "").ToString();
            //        var isMarked = !String.IsNullOrWhiteSpace(value);
            //        if (isMarked)
            //        {

            //            mergeGroups(col_group, row_group); 

            //        }
            //    }
            //}

            var clone = dtMethodsByClass.Copy();
            
            var rows = clone.Rows.OfType<DataRow>().ToList();
            //var cols = clone.Columns.OfType<DataColumn>().Where(c=> c.ColumnName != COL_METHOD_NAME).ToList();

            var groups = new List<Tuple<List<DataRow>, List<DataColumn>>>();
            var nonEmptyrows = rows.Where(r => r.ItemArray.Any(o => !String.IsNullOrWhiteSpace((o ?? "").ToString()))).ToList();

            
            try
            {
                foreach (var row in nonEmptyrows)
                {
                    bool wasRowHandled = groups.Any(g => g.Item1.Contains(row));

                    if (wasRowHandled)
                        continue;
                    this.GetConnectedColumns(row, groups, clone);
                }


                var orderedGroups = groups.OrderByDescending(g => g.Item2.Count).ToList();
                var sortVal =0;

                foreach (var g in orderedGroups)
                {
                    var row = g.Item1;
                    var cols = g.Item2;

                    foreach (var col in cols)
                    {
                        //var retCol = ret.Columns.OfType<DataColumn>().FirstOrDefault(c => c.ColumnName == col.ColumnName);
                        //retCol.SetOrdinal(0);
                        col.SetOrdinal(0);
                    }

                    foreach (var r in rows)
                    {
                        r[COL_SORT_VALUE] = --sortVal;//put at position 0...  
                       
                    }
                        
                }

                clone.DefaultView.Sort = $"{COL_SORT_VALUE}";
                clone = clone.DefaultView.ToTable();
            }
            catch (Exception ex){ clone = dtMethodsByClass; }


           
        

            return clone;


        }

        private void GetConnectedColumns(DataRow row, List<Tuple<List<DataRow>, List<DataColumn>>> groupsAggregator, DataTable dt)
        {
            var group = groupsAggregator.Where(g => g.Item1.Contains(row)).FirstOrDefault();

            if (group == null)
            {
                group = new Tuple<List<DataRow>, List<DataColumn>>(new List<DataRow>(), new List<DataColumn>());
                group.Item1.Add(row);
                groupsAggregator.Add(group);
            }
            var unVisitedCols = dt.Columns.OfType<DataColumn>()
                                                        .Where(c=> c.ColumnName != COL_METHOD_NAME && c.ColumnName !=COL_SORT_VALUE)
                                                        .Where(c => !groupsAggregator.Any(t => t.Item2.Contains(c))).ToList();
            var connectedCols = unVisitedCols.Where(c => !String.IsNullOrWhiteSpace((row[c] ?? "").ToString())).ToList();

            group.Item2.AddRange(connectedCols);
           
            foreach (var col in connectedCols)
            {
                this.GetConnectedRows(col, groupsAggregator, dt);
            }
            
         
         
        }

        private void GetConnectedRows(DataColumn col, List<Tuple<List<DataRow>, List<DataColumn>>> groupsAggregator, DataTable dt)
        {
            var group = groupsAggregator.Where(g => g.Item2.Contains(col)).FirstOrDefault();
            if (group == null)
            {
                group = new Tuple<List<DataRow>, List<DataColumn>>(new List<DataRow>(), new List<DataColumn>());
                group.Item2.Add(col);
                groupsAggregator.Add(group);
            }
            var unVisitedRows = dt.Rows.OfType<DataRow>().Where(r => !groupsAggregator.Any(t => t.Item1.Contains(r))).ToList();
            var connectedRows = unVisitedRows.Where(r => !String.IsNullOrWhiteSpace((r[col] ?? "").ToString())).ToList();
            group.Item1.AddRange(connectedRows);

            foreach (var row in connectedRows)
            {
                this.GetConnectedColumns(row, groupsAggregator, dt);
            }
        }

        private DataTable GenerateModularityMatrix()
        {

            var classNames = this._classedWithMethods.classessWithMethods.Select(cm => cm.ClassName).Distinct().ToList();
            var methods = this._classedWithMethods.classessWithMethods.SelectMany(cm => cm.GetPublicMemberNames()).Distinct().ToList();
            
            
            var clmNames = classNames;

            var dt = new DataTable();

            dt.Columns.Add(COL_METHOD_NAME, typeof(string));
            dt.Columns.Add(COL_SORT_VALUE, typeof(int));
            foreach (var cName in clmNames)
            {
                dt.Columns.Add(cName, typeof(string));
            }

            foreach (var methodName in methods)
            {
                var row = dt.NewRow();
                row[COL_METHOD_NAME] = methodName;
                row[COL_SORT_VALUE] = int.MaxValue;
                foreach (var className in clmNames)
                {
                    var classWithMethods = this._classedWithMethods.GetClassMethods(className);
                    var currMethods = classWithMethods.GetPublicMemberNames();


                    string val = currMethods.Contains(methodName) ? "1" : "";
                    row[className] = val;
                }
                dt.Rows.Add(row);
            }

            //dt.PrimaryKey = new DataColumn[] { dt.Columns[COL_METHOD_NAME] };
            return dt;
        }
    }
}