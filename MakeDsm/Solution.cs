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
using System.Text.RegularExpressions;

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
            
            List<ClassDeclarationSyntax> solutionTypes = this.GetSolutionClassDeclarations();
            
            var res = solutionTypes.ToDictionary(t => t,
                                          t =>
                                          {
                                              var allrefernces = new List<ReferencedSymbol>();
                                              var projects = this.Projects;
                                              foreach (var p in projects)
                                              {
                                                  var references = new List<ReferencedSymbol>();

                                                  try
                                                  {


                                                      //var compilation = CSharpCompilation.Create("MyCompilation", new SyntaxTree[] { t.p.SyntaxTree }, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
                                                      var compilation = p.GetCompilationAsync().Result;
                                                      var semanticModel = compilation.GetSemanticModel(t.SyntaxTree);
                                                      var classSymbols = semanticModel.GetDeclaredSymbol(t);

                                                       references = SymbolFinder.FindReferencesAsync(classSymbols, this._solution).Result.ToList();
                                                      foreach (var r in references)
                                                      {
                                                          var loc = SymbolFinder.FindSourceDefinitionAsync(r.Definition, this._solution).Result;
                                                      }
                                                  }
                                                  catch (Exception)
                                                  {

                                                      //still testing
                                                  }
                                                  allrefernces.AddRange(references);
                                              }
                                              
                                              return allrefernces;
                                          });



#region Testing
            //var resAAA = solutionTypes.ToDictionary(t => t,
            //                              t =>
            //                              {
            //                                  var references = new List<ReferencedSymbol>();
            //                                  return references;
            //                                  var allDocuments = this.Projects.SelectMany(p => p.Documents);
            //                                  var allTrees = allDocuments.Select(d => d.GetSyntaxTreeAsync().Result);
            //                                  var compilation = CSharpCompilation.Create("MyCompilation", allTrees, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
            //                                  var semanticModels = allTrees.Select(tree => compilation.GetSemanticModel(tree));

            //                                  try
            //                                  {
            //                                      var classSymbols = semanticModels.Select(model => model.GetDeclaredSymbol(t));
            //                                      references = classSymbols.SelectMany(cs => SymbolFinder.FindReferencesAsync(cs, _solution).Result).ToList();

            //                                  }
            //                                  catch (Exception)
            //                                  {
            //                                      // throw;
            //                                  }

            //                                  return references;
            //                              });  
#endregion

#if PRINT_DETAILS
            var a = res.OrderByDescending(p => p.Value.Count).ToList();
            for (int idx = 0; idx < a.Count; idx++)
            {
                var curr = a[idx];
                var ty = curr.Key.ToString();

                Debug.WriteLine($"@@@'(X{curr.Value.Count}) {ty}' \n Apperas in:\n" + String.Join("\n", curr.Value.Select(v => v.Definition.ContainingType)));


            }

#endif


            return new RoslynDenpendencies(res);
        }

        private List<ClassDeclarationSyntax> GetSolutionClassDeclarations()
        {
            var solutionTypes = new List<ClassDeclarationSyntax>();
            foreach (var project in _solution.Projects)
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
                solutionTypes.AddRange(projectTypes);
            }

            return solutionTypes;
        }





    }

    
}