using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MakeDsm
{
    public class Denpendencies: IDenpendencies
    {
        
        public IReadOnlyDictionary<string, List<string>> DependencyDictionary { get; }

        internal Denpendencies(IDictionary<string, List<string>> dic)
        {
            this.DependencyDictionary = new ReadOnlyDictionary<string, List<string>>(dic);
        }
    }
}