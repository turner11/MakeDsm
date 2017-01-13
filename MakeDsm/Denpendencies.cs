using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MakeDsm
{
    public class Denpendencies: IDenpendencies
    {
        
        public IReadOnlyDictionary<string, List<string>> DependencyDictionary { get; }

        public ClassessWithMethods ClassessWithMethods
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        internal Denpendencies(IDictionary<string, List<string>> dic)
        {
            this.DependencyDictionary = new ReadOnlyDictionary<string, List<string>>(dic);
        }

        public ClassWithMethods GetClassMethods(string classname)
        {
            //Ya. I preatty much left the C when roslyn kicked in...
            throw new NotImplementedException();
        }
    }
}