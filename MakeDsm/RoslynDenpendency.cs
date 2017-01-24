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
        public string Name { get { return this._classWithReferences.ClassName; } }
        public IReadOnlyCollection<TypeDeclarationSyntax> ReferenceingTypes { get; }
        private IReadOnlyCollection<ReferencedSymbol> _references { get{ return this._classWithReferences.References; } }
        internal RoslynDenpendency(ClassWithReferencess classWithReferences)
        {
            this._classWithReferences = classWithReferences;
            this.ReferenceingTypes = this.GetReferenceingTypes().AsReadOnly();

            Debug.Print("@@@@@@  "+this.ToString());
            
        }

        private List<TypeDeclarationSyntax> GetReferenceingTypes()
        {
            var ret = new List<TypeDeclarationSyntax>();


            foreach (var r in this._references)
            {
                var locations = r.Locations;
                foreach (var l in locations)
                {
                    var tree = l.Location.SourceTree;

                    var declaringType_Classes = tree.GetRoot().DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>();
                    var declaringType_Interface = tree.GetRoot().DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>();
                    var declaringType = (TypeDeclarationSyntax)declaringType_Classes?.LastOrDefault() ?? declaringType_Interface.LastOrDefault();
                    ret.Add(declaringType);
                    //var dt = declaringType.ToString();
                    //var dti = declaringType.Identifier.ToString();

                }
            }

            return ret.Distinct().ToList();
        }

        public override string ToString()
        {
            return $"{this._classWithReferences.ClassName}: " + String.Join(", ", this.ReferenceingTypes.Select(t => t.Identifier));
        }
    }
}
