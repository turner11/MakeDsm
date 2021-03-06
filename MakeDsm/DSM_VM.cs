﻿using System;
using System.Data;
using System.Linq;

namespace MakeDsm
{
    internal class DSM_VM
    {
        public const string COL_NAME = "COLNAME";
        public const string COL_SORT = "COLSORT";
        private IDenpendencies DependeciesModel { get; }

        public DataTable DSM { get; }

        public DSM_VM(IDenpendencies dependeciesModel)
        {
            this.DependeciesModel = dependeciesModel;
            this.DSM = this.GenerateDSM();
        }

        private DataTable GenerateDSM()
        {
            var dic = this.DependeciesModel.DependencyDictionary;
            var dependencies = dic.OrderBy(p => p.Value.Count);
            var clmNames = dic.Keys.ToList();

            var dt = new DataTable();
            dt.Columns.Add(COL_NAME, typeof(string));
            dt.Columns.Add(COL_SORT, typeof(int));
            foreach (var cName in clmNames)
            {
                dt.Columns.Add(cName, typeof(string));
            }

            foreach (var d in dependencies)
            {
                var row = dt.NewRow();
                var moduleName = d.Key;
                row[COL_NAME] = moduleName;
                row[COL_SORT] = dt.Columns.IndexOf(moduleName);

                foreach (var cName in clmNames)
                {
                    var deps = d.Value;

                    string val = "";
                    if (cName == moduleName)
                    {
                        val = ".";
                    }
                    else if(deps.Contains(cName) )
                    {
                        val= "1";
                    }
                    row[cName] = val;
                    //var val = deps.Contains(cName)? 1:0;
                    //row[cName] = val;
                }



                dt.Rows.Add(row);
            }

            dt.DefaultView.Sort = $"{COL_SORT}";
            dt = dt.DefaultView.ToTable();
            return dt;   
        }

        public ModularityMatrixVM GetModularityMatrix()
        {
            return new ModularityMatrixVM(this.DependeciesModel.ClassessWithMethods);
        }
    }
}