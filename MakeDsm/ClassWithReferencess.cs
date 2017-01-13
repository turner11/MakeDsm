using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm
{
    internal class ClassWithReferencess:WrapperWithClass
    {
       
        internal IReadOnlyCollection<ReferencedSymbol> References { get; }

        public ClassWithReferencess(ClassDeclarationSyntax @class, IList<ReferencedSymbol> references):base(@class)
        {
            this.References = references.ToList().AsReadOnly();
            

#if PRINT_DETAILS
                Debug.WriteLine($"@@@@@@ '{this.Name }' (X{this.References.Count}) \n Apperas in:\n" + String.Join("\n", this.References.Select(v => v.Definition.ContainingType).Distinct()));
#endif
            foreach (var r in this.References)
            {
                var s = r.Definition.ContainingType;

                foreach (var l in r.Locations)
                {

                    var t = l.Location.SourceTree;
                    var loc = t.ToString();
                    var a = t.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().LastOrDefault();
                    var at = a?.ToString();
                    //l.Location.SourceSpan.ToString();
                }
            }
        }


       

        public IList<string> GetReferencesNames()
        {
            return this.References.Select(r => r.Definition.Name).ToList();
        }


        public override string ToString()
        {
            return this.ClassName;
        }
    }
}
