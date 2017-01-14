using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace MakeDsm
{
    internal class ModularityMatrixVM
    {
        public const string COL_METHOD_NAME ="METHOD NAME";
        public const string COL_SORT_VALUE ="SORT VALUE";

        static ReadOnlyCollection<string> NonDataColumns { get{ return new List<string> { COL_METHOD_NAME,COL_SORT_VALUE}.AsReadOnly(); } }
        private readonly ClassessWithMethods _classedWithMethods;
        public DataTable ModularityMatrix { get; }

        public ModularityMatrixVM(ClassessWithMethods classessWithMethods)
        {
            var aligner = new DataTableAligner();

            this._classedWithMethods = classessWithMethods;
            var dtMethodsByClass = this.GenerateModularityMatrix();

            this.ModularityMatrix = aligner.MakeBlockDiagonalTable(dtMethodsByClass);
            return;
            //testing...
            var dt = new DataTable();
            dt.TableName = "Test";
            int colCount = 10;
            dt.Columns.Add(COL_METHOD_NAME, typeof(string));
            dt.Columns.Add(COL_SORT_VALUE, typeof(int));
            for (int i = 0; i < colCount; i++)
            {
                dt.Columns.Add(i.ToString(), typeof(int));
                
            }

            Random rnd = new Random();

            for (int i = 0; i < colCount; i++)
            {
                var r = dt.NewRow();
                //var idx = 2+((i + 2) % (colCount - 1));
                var idx = 2 + rnd.Next(0, colCount - 1);
                r[idx] = 1;
                r[COL_METHOD_NAME] = "COL "+i;
                dt.Rows.Add(r);
            }

            string before = dt.AsString();
            

            this.ModularityMatrix = aligner.MakeBlockDiagonalTable(dt);

            //put non data at begining
            NonDataColumns.ToList().ForEach(c => this.ModularityMatrix.Columns[c].SetOrdinal(0));
            

            var after = this.ModularityMatrix.DefaultView.ToTable().AsString();
            var combined = before + "\n\n" + after;

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


            Func<DataRow, DataColumn, bool> IsEmpty = (r, c) => String.IsNullOrWhiteSpace(r[c].ToString());
            var dataColumns = dt.Columns.Cast<DataColumn>().Where(c=> !NonDataColumns.Contains(c.ColumnName)).ToArray();

            //remove empty rows
            foreach (var dr in dt.Rows.Cast<DataRow>().ToArray())
            {

                if (dataColumns.All(c => IsEmpty(dr, c)))
                    dt.Rows.Remove(dr);
            }

            //remove empty columns
            foreach (var clm in dataColumns)
            {
                if (dt.AsEnumerable().All(dr => IsEmpty(dr, clm)))
                    dt.Columns.Remove(clm);
            }

          
            //dt.PrimaryKey = new DataColumn[] { dt.Columns[COL_METHOD_NAME] };
            return dt;
        }


        class DataTableAligner
        {
            internal DataTable MakeBlockDiagonalTable(DataTable dtMethodsByClass)
            {
#region Attempt 1
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
# endregion

                var clone = dtMethodsByClass.Copy();
                clone.DefaultView.Sort = $"{COL_SORT_VALUE}";

                var rows = clone.Rows.OfType<DataRow>().ToList();
                //var cols = clone.Columns.OfType<DataColumn>().Where(c=> c.ColumnName != COL_METHOD_NAME).ToList();

                var nonEmptyrows = rows.Where(r => r.ItemArray.Any(o => !String.IsNullOrWhiteSpace((o ?? "").ToString()))).ToList();
                var groupsAggregator = new List<TableGrouop>();
                try
                {
                    foreach (var row in nonEmptyrows)
                    {
                        bool wasRowHandled = groupsAggregator.Any(g => g.Contains(row));

                        if (wasRowHandled)
                            continue;
                        this.GetConnectedColumns(row, groupsAggregator, clone);
                        var str = groupsAggregator.Last().ToString();
                       
                    }


                    var orderedGroups = groupsAggregator.OrderByDescending(g => g.columns.Count).ToList();
                    var sortVal = 0;

                    foreach (var g in orderedGroups)
                    {
                        var str = g.ToString();
                        var gRows = g.rows;
                        var gCols = g.columns;

                        foreach (var col in gCols)
                        {
                            //var retCol = ret.Columns.OfType<DataColumn>().FirstOrDefault(c => c.ColumnName == col.ColumnName);
                            //retCol.SetOrdinal(0);
                            col.SetOrdinal(0);
                        }

                        foreach (var r in gRows)
                        {
                            r[COL_SORT_VALUE] = --sortVal;//put at position 0...  

                        }

                    }


                    clone = clone.DefaultView.ToTable();
                }
                catch (Exception ex) { clone = dtMethodsByClass; }

                return clone;


            }
            private void GetConnectedColumns(DataRow row, List<TableGrouop> groupsAggregator, DataTable dt)
            {
                var group = groupsAggregator.Where(g => g.rows.Contains(row)).FirstOrDefault();

                if (group == null)
                {
                    group = new TableGrouop();
                    group.rows.Add(row);
                    groupsAggregator.Add(group);
                }
                var unVisitedCols = dt.Columns.OfType<DataColumn>()
                                                            .Where(c => !NonDataColumns.Contains(c.ColumnName)) //igonr non data columns
                                                            .Where(c => !groupsAggregator.Any(t => t.columns.Contains(c))).ToList();
                var connectedCols = unVisitedCols.Where(c => !String.IsNullOrWhiteSpace((row[c] ?? "").ToString())).ToList();

                group.columns.AddRange(connectedCols);

                foreach (var col in connectedCols)
                {
                    this.GetConnectedRows(col, groupsAggregator, dt);
                }



            }

            private void GetConnectedRows(DataColumn col, List<TableGrouop> groupsAggregator, DataTable dt)
            {
                var group = groupsAggregator.Where(g => g.Contains(col)).FirstOrDefault();
                if (group == null)
                {
                    group = new TableGrouop();
                    group.columns.Add(col);
                    groupsAggregator.Add(group);
                }
                var unVisitedRows = dt.Rows.OfType<DataRow>().Where(r => !groupsAggregator.Any(t => t.Contains(r))).ToList();
                var connectedRows = unVisitedRows.Where(r => !String.IsNullOrWhiteSpace((r[col] ?? "").ToString())).ToList();
                group.rows.AddRange(connectedRows);

                foreach (var row in connectedRows)
                {
                    this.GetConnectedColumns(row, groupsAggregator, dt);
                }
            }


            private class TableGrouop
            {
                internal readonly List<DataRow> rows;
                internal readonly List<DataColumn> columns;
                internal void Add(DataRow row) => rows.Add(row);
                internal void Add(DataColumn col) => columns.Add(col);


                internal bool Contains(DataRow row) => rows.Contains(row);
                internal bool Contains(DataColumn col) => columns.Contains(col);

                internal int RowsCount { get { return this.rows.Count;  } }
                internal int ColumnsCount { get { return this.columns.Count; } }

                public TableGrouop()
                {
                    this.rows = new List<DataRow>();
                    this.columns = new List<DataColumn>();
                }

                public override string ToString()
                {
                    var strRows = string.Join(", ", rows.Select(r=> r[COL_METHOD_NAME]));
                    var strCols = string.Join(", ", columns.Select(c=> c.ColumnName));
                    return $"{rows.Count} Rows : {strRows}\t {columns.Count} Columns: {strCols}";
                }

            }
        }
    }
}