using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm
{
    internal class ClassWithReferencess
    {
        internal ClassDeclarationSyntax ClassDeclarationSyntax { get; }
        internal IReadOnlyCollection<ReferencedSymbol> References { get; }

        public string Name { get { return this.ClassDeclarationSyntax.GetClassWithNameSpace(); } }

        public ClassWithReferencess(ClassDeclarationSyntax @class, IList<ReferencedSymbol> references)
        {
            this.ClassDeclarationSyntax = @class;
            this.References = references.ToList().AsReadOnly();
            

#if PRINT_DETAILS
                Debug.WriteLine($"@@@@@@ '{this.Name }' (X{this.References.Count}) \n Apperas in:\n" + String.Join("\n", this.References.Select(v => v.Definition.ContainingType).Distinct()));
#endif
            foreach (var r in this.References)
            {
                var s = r.Definition.ContainingType;

                //foreach (var sr in s)
                //{
                //    var t = r.Locations.First().Location.SourceTree.ToString();
                //    var a = sr.GetSyntax();
                //}
                //FindSourceDefinitionAsync
                foreach (var l in r.Locations)
                {

                    var t = l.Location.SourceTree;
                    var loc = t.ToString();
                    var a = t.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();
                    var at = a.ToString();
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
            return this.Name;
        }
    }
}
