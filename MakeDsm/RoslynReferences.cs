#if DEBUG
#define PRINT_DETAILSX
#endif
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using System.Diagnostics;

namespace MakeDsm
{
    internal class RoslynReferences : IDenpendencies
    {
        public ClassessWithMethods ClassessWithMethods { get; }

        public IReadOnlyCollection<RoslynDenpendency> ClassessWithReferences { get; }
        
        public IReadOnlyDictionary<string, List<string>> DependencyDictionary { get; }

        public RoslynReferences(IList<RoslynDenpendency> dependencies, ClassessWithMethods classesWithMethods)
        {
            this.ClassessWithReferences = dependencies.ToList().AsReadOnly();
            this.ClassessWithMethods = classesWithMethods;


            var depDic = this.ClassessWithReferences.ToDictionary(dp => dp.Name,
                                                 dp => dp.ReferenceingTypes.Select(rt=> rt.GetClassWithNameSpace()).ToList());

            this.DependencyDictionary = depDic;


#if PRINT_DETAILS

            var s = this.GetFullString();
            Debug.WriteLine(s);
#endif
        }

        private string GetFullString()
        {
            var lines = DependencyDictionary.OrderByDescending(p=> p.Value.Count).Select(p => $"{p.Key}: \t\t\t"+String.Join(",\t",p.Value));
            var s = String.Join(Environment.NewLine, lines);
            return s;
        }

        public ClassWithMethods GetClassMethods(string classname)
        {
            return this.ClassessWithMethods.GetClassMethods(classname);
        }
    }
}