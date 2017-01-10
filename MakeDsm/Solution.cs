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

                    try
                    {
                        var semanticModel = compilation.GetSemanticModel(t.SyntaxTree);
                        var classSymbols = semanticModel.GetDeclaredSymbol(t);

                        references = SymbolFinder.FindReferencesAsync(classSymbols, this._solution).Result.ToList();
                        foreach (var r in references)
                        {
                            var loc = SymbolFinder.FindSourceDefinitionAsync(r.Definition, this._solution).Result;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                        //still testing
                    }
                    res[t] = references;
                }
            }

            //this step is just for breaking the process to few more comprehndable (and readble) steps
            List<ClassWithReferencess> classesWithReferences = 
                res.Select(p => new ClassWithReferencess(p.Key, p.Value)).ToList();

            //partial classes will apear multiple times - Unify them!
            classesWithReferences =
                classesWithReferences.GroupBy(cr => cr.Name)
                .Select(g => new ClassWithReferencess(g.First().ClassDeclarationSyntax,g.SelectMany(c=>c.References).ToList()))
                .ToList();

            List<RoslynDenpendency> dependencies = 
                classesWithReferences.Select(cr => new RoslynDenpendency(cr)).ToList();



            return new RoslynReferences(dependencies);
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