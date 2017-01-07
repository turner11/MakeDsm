using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MakeDsm
{
    internal class RoslynDenpendencies : IDenpendencies
    {
        
        private ReadOnlyCollection<ClassWithReferencess> ClassessWithReferences { get; }
        
        public IReadOnlyDictionary<string, List<string>> DependencyDictionary { get; }

        public RoslynDenpendencies(Dictionary<ClassDeclarationSyntax, List<ReferencedSymbol>> res)
        {
           this.ClassessWithReferences = res.Select(p => new ClassWithReferencess(p.Key, p.Value))
                                                .ToList().AsReadOnly();
            //partial classess will generate duplicate keys, we should uinfy them
            var groupedDeps = ClassessWithReferences.GroupBy(c => c.Name);
            var depDic = groupedDeps.ToDictionary(g => g.First().Name,
                                                 g => g.SelectMany(c => c.GetReferencesNames()).ToList());

            this.DependencyDictionary = depDic;
                

#if DEBUG
            
            var byRefs = this.DependencyDictionary.ToList().OrderByDescending(p => p.Value.Count).ToList();
            var rawByRefs = this.ClassessWithReferences.OrderByDescending(c => c.References.Count).ToList();

            var a = rawByRefs.SelectMany(c => c.References.Where(r => r.Locations.Count() > 0)).ToList();
            
            1.ToString();

#endif
        }


       


        internal class ClassWithReferencess
        {
            internal ClassDeclarationSyntax ClassDeclarationSyntax { get; }
            internal IReadOnlyCollection<ReferencedSymbol> References { get; }

            public readonly string Name;

            public ClassWithReferencess(ClassDeclarationSyntax @class, IList<ReferencedSymbol> references)
            {
                this.ClassDeclarationSyntax = @class;
                this.References = references.ToList().AsReadOnly();
                this.Name = this.GetClassWithNameSpace(this.ClassDeclarationSyntax);

                foreach (var r in this.References)
                {
                    //FindSourceDefinitionAsync
                    foreach (var l in r.Locations)
                    {
                        var loc = l.Document.GetSemanticModelAsync().Result.GetEnclosingSymbol(0);
                    }
                }
            }


            private string GetClassWithNameSpace(ClassDeclarationSyntax sn)
            {
                string ret = sn.Identifier.ToString();
                NamespaceDeclarationSyntax namespaceDeclarationSyntax = null;
                if (SyntaxNodeHelper.TryGetParentSyntax(sn, out namespaceDeclarationSyntax))
                {
                    var namespaceName = namespaceDeclarationSyntax.Name.ToString();
                    var fullClassName = namespaceName + "." + ret;
                    ret = fullClassName;
                }

                return ret;
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
}