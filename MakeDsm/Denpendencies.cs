using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MakeDsm
{
    public class Denpendencies
    {
        
        public  readonly IReadOnlyDictionary<string, List<string>> DependencyDictionary;

        internal Denpendencies(IDictionary<string, List<string>> dic)
        {
            this.DependencyDictionary = new ReadOnlyDictionary<string, List<string>>(dic);
        }
    }
}