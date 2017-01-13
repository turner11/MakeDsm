using System.Data;
using System.Linq;

namespace MakeDsm
{
    internal class ModularityMatrixVM
    {
        public const string COL_METHOD_NAME ="METHOD NAME";
        private readonly ClassessWithMethods _classedWithMethods;

        public ModularityMatrixVM(ClassessWithMethods classessWithMethods)
        {
            this._classedWithMethods = classessWithMethods;
            this.ModularityMatrix = this.GenerateModularityMatrix();
        }

    

        public object ModularityMatrix { get;  }

        private DataTable GenerateModularityMatrix()
        {

            var classNames = this._classedWithMethods.classessWithMethods.Select(cm => cm.ClassName).Distinct().ToList();
            var methods = this._classedWithMethods.classessWithMethods.SelectMany(cm => cm.GetPublicMemberNames()).Distinct().ToList();
            
            
            var clmNames = classNames;

            var dt = new DataTable();

            dt.Columns.Add(COL_METHOD_NAME, typeof(string));
            foreach (var cName in clmNames)
            {
                dt.Columns.Add(cName, typeof(string));
            }

            foreach (var methodName in methods)
            {
                var row = dt.NewRow();
                row[COL_METHOD_NAME] = methodName;
                foreach (var className in clmNames)
                {
                    var classWithMethods = this._classedWithMethods.GetClassMethods(className);
                    var currMethods = classWithMethods.GetPublicMemberNames();


                    string val = currMethods.Contains(methodName) ? "1" : "";
                    row[className] = val;
                }
                dt.Rows.Add(row);
            }

            //dt.DefaultView.Sort = $"{COL_SORT}";
            //dt = dt.DefaultView.ToTable();
            return dt;
        }
    }
}