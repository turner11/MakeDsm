using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakeDsm
{
    internal partial class MakeDsm_CS : MakeDsmService
    {
       Solution _solution { get; }
        public MakeDsm_CS(string path) : base(path)
        {
            this._solution = new Solution(this.MakeFilePath);

        }

      

        protected override Denpendencies GetDependencies()
        {
            var dic = this._solution.Projects.ToDictionary(p => p.Name,p=> p.Dependencies.SelectMany(d=>d.Project.Dependencies));
            dic.ToString();
            return null;
           
        }
    }
}
