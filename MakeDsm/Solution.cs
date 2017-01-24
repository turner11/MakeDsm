#if DEBUG
#define PRINT_DETAILS
#endif

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;



namespace MakeDsm
{
    internal partial class DotNetSolution
    {
        public string Path { get; }
        private IList<Project> Projects { get { return this._solution.Projects.ToList(); } }

        private MSBuildWorkspace _msWorkspace;
        private Microsoft.CodeAnalysis.Solution _solution;


        public DotNetSolution(string slnPath)
        {
            this.Path = slnPath;
            //string solutionPath = @"C:\Users\...\PathToSolution\MySolution.sln";
            string solutionPath = slnPath;


            this._msWorkspace = MSBuildWorkspace.Create();
            this._solution = _msWorkspace.OpenSolutionAsync(solutionPath).Result;
            

        }

        internal IDenpendencies GetDependencies()
        {

            Dictionary<Project, List<ClassDeclarationSyntax>> solutionTypes = this.GetSolutionClassDeclarations();

            var res = new Dictionary<ClassDeclarationSyntax, List<ReferencedSymbol>>();
            foreach (var pair in solutionTypes)
            {
                Project proj = pair.Key;
                List<ClassDeclarationSyntax> types = pair.Value;
                var compilation = proj.GetCompilationAsync().Result;
                foreach (var t in types)
                { 
                    var references = new List<ReferencedSymbol>();
                    
                    var semanticModel = compilation.GetSemanticModel(t.SyntaxTree);
                    var classSymbols = semanticModel.GetDeclaredSymbol(t);

                    references = SymbolFinder.FindReferencesAsync(classSymbols, this._solution).Result.ToList();                  
                   
                    res[t] = references;
                }
            }

            //this step is just for breaking the process to few more comprehndable (and readble) steps
            List<ClassWithReferencess> classesWithReferences = 
                res.Select(p => new ClassWithReferencess(p.Key, p.Value)).ToList();

            //partial classes will apear multiple times - Unify them!
            classesWithReferences =
                classesWithReferences.GroupBy(cr => cr.ClassName)
                .Select(g => new ClassWithReferencess(g.First().ClassDeclarationSyntax,g.SelectMany(c=>c.References).ToList()))
                .ToList();

            List<RoslynDenpendency> dependencies = 
                classesWithReferences.Select(cr => new RoslynDenpendency(cr)).ToList();

            var classessWithNames = this.GetMethodsByClass();

            var classMethods = dependencies.Select(d=> d._classWithReferences.ClassDeclarationSyntax);
            return new RoslynReferences(dependencies, classessWithNames);
        }

        internal ClassessWithMethods GetMethodsByClass()
        {

            var res = new Dictionary<ClassDeclarationSyntax, List<MemberDeclarationSyntax>>();
            Dictionary<Project, List<ClassDeclarationSyntax>> solutionTypes = this.GetSolutionClassDeclarations();


           
            //-See more at: http://www.filipekberg.se/2011/10/21/getting-all-methods-from-a-code-file-with-roslyn/#sthash.5RgyiZfG.dpuf
            foreach (var pair in solutionTypes)
            {
                Project proj = pair.Key;
                List<ClassDeclarationSyntax> types = pair.Value;
                var compilation = proj.GetCompilationAsync().Result;
                foreach (var t in types)
                {
                    var semanticModel = compilation.GetSemanticModel(t.SyntaxTree);
                    //var classSymbols = semanticModel.GetDeclaredSymbol(t);

                   //var methods = t.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
                   //var properties = t.DescendantNodes().OfType<PropertyDeclarationSyntax>().ToList();


                    var mathodsAndProperties = new List<MemberDeclarationSyntax>();
                    //mathodsAndProperties.AddRange(methods);
                    //mathodsAndProperties.AddRange(properties);

                    var typeT = (ITypeSymbol)semanticModel.GetDeclaredSymbol(t);
                    var members = typeT.GetBaseTypesAndThis().SelectMany(n => n.GetMembers()).ToList();
                    var membersSyntax = members.SelectMany(m => m.DeclaringSyntaxReferences.Select(r => r.GetSyntax())).ToList();
                    
                    var methods = membersSyntax.OfType<MethodDeclarationSyntax>().ToList();
                    var properties = membersSyntax.OfType<PropertyDeclarationSyntax>().ToList();

                    mathodsAndProperties.AddRange(methods);
                    mathodsAndProperties.AddRange(properties);


                    res[t] = mathodsAndProperties;
                    
                }
                
            }

            var classessWithMethods = res.Select(p => new ClassWithMethods(p.Key, p.Value)).ToList();
            return new ClassessWithMethods(classessWithMethods);


        }

        private Dictionary<Project,List<ClassDeclarationSyntax>> GetSolutionClassDeclarations()
        {
            var projs = this._solution.Projects;

            var solutionTypes = projs.ToDictionary(p=> p,p=> new List<ClassDeclarationSyntax>());
            foreach (var project in projs)
            {
                var projectTypes = new List<ClassDeclarationSyntax>();
                foreach (var document in project.Documents)
                {


                    var tree = document.GetSyntaxRootAsync().Result;
                    var typeStatements = tree.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                    //var typeStatement = typeStatements.FirstOrDefault()?.ToString();
                    //var methodDecStatements = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
                    //var ifStatements = tree.DescendantNodes().OfType<IfStatementSyntax>().ToList();
                    //var a = methodDecStatements.FirstOrDefault()?.ToString();
                    //var b = ifStatements.FirstOrDefault()?.ToString();

                    projectTypes.AddRange(typeStatements);

                }
                Debug.WriteLine($"In project '{project.Name}', Found the following types:\n" + String.Join("\n", projectTypes.Select(t => "\t"+ t.Identifier.Text)));
                solutionTypes[project].AddRange(projectTypes);
            }

            return solutionTypes;
        }





    }

    
}