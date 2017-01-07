using System.Collections.Generic;

namespace MakeDsm
{
    public interface IDenpendencies
    {
        IReadOnlyDictionary<string, List<string>> DependencyDictionary { get; }
    }
}