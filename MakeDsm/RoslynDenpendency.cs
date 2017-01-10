using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm
{
    public class RoslynDenpendency
    {

        internal ClassWithReferencess _classWithReferences { get; }
        public ClassDeclarationSyntax Type { get { return this._classWithReferences.ClassDeclarationSyntax; } }
        public string Name { get { return this._classWithReferences.Name; } }
        public IReadOnlyCollection<ClassDeclarationSyntax> ReferenceingTypes { get; }
        private IReadOnlyCollection<ReferencedSymbol> _references { get{ return this._classWithReferences.References; } }
        internal RoslynDenpendency(ClassWithReferencess classWithReferences)
        {
            this._classWithReferences = classWithReferences;
            this.ReferenceingTypes = this.GetReferenceingTypes().AsReadOnly();

            Debug.Print("@@@@@@  "+this.ToString());
            
        }

        private List<ClassDeclarationSyntax> GetReferenceingTypes()
        {
            var ret = new List<ClassDeclarationSyntax>();


            foreach (var r in this._references)
            {
                var locations = r.Locations;
                foreach (var l in locations)
                {
                    var tree = l.Location.SourceTree;
                    var declaringType = tree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().Last();
                    ret.Add(declaringType);
                    var dt = declaringType.ToString();
                    var dti = declaringType.Identifier.ToString();

                }
            }

            return ret.Distinct().ToList();
        }

        public override string ToString()
        {
            return $"{this._classWithReferences.Name}: " + String.Join(", ", this.ReferenceingTypes.Select(t => t.Identifier));
        }
    }
}
