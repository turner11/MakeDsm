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
    internal partial class Solution
    {
        public readonly IReadOnlyCollection<Project> Projects;
        public string Path { get; }
        private readonly string Content;

        public Solution(string slnPath)
        {
            this.Path = slnPath;
            this.Content = System.IO.File.ReadAllText(slnPath);

            //string solutionPath = @"C:\Users\...\PathToSolution\MySolution.sln";
            string solutionPath = slnPath;

            var msWorkspace = MSBuildWorkspace.Create();

            var solution = msWorkspace.OpenSolutionAsync(solutionPath).Result;
            foreach (var project in solution.Projects)
            {
                var projectTypes = new List<ClassDeclarationSyntax>();
                foreach (var document in project.Documents)
                {
                    Debug.WriteLine(project.Name + "\t\t\t" + document.Name);
                    var tree = document.GetSyntaxRootAsync().Result;
                    var methodDecStatements = tree.DescendantNodes().OfType<MethodDeclarationSyntax>().ToList();
                    var ifStatements = tree.DescendantNodes().OfType<IfStatementSyntax>().ToList();
                    var typeStatements = tree.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();
                    //var a = methodDecStatements.FirstOrDefault()?.ToString();
                    //var b = ifStatements.FirstOrDefault()?.ToString();
                    var typeStatement = typeStatements.FirstOrDefault()?.ToString();
                    
                    projectTypes.AddRange(typeStatements);


                  
                }
                //TODO: Ithin I am looking only in the declaration ( compilation.GetSemanticModel(t.SyntaxTree);)
                var res =  projectTypes.ToDictionary(t => t,
                                          t =>
                                          {
                                              var compilation = CSharpCompilation.Create("MyCompilation", new SyntaxTree[] { t.SyntaxTree }, new[] { MetadataReference.CreateFromFile(typeof(object).Assembly.Location) });
                                              var semanticModel = compilation.GetSemanticModel(t.SyntaxTree);
                                              var classSymbols = semanticModel.GetDeclaredSymbol(t);
                                              var references = SymbolFinder.FindReferencesAsync(classSymbols, solution).Result;
                                              return references.ToList();
                                              });

                var a = res.OrderByDescending(p => p.Value.Count).ToList();
                for (int idx = 0; idx < a.Count; idx++)
                {
                    var curr = a[idx];
                    var ty = curr.Key.ToString();

                    Debug.WriteLine($"@@@'(X{curr.Value.Count}) {ty}' \n Apperas in:\n" + String.Join("\n",curr.Value.Select(v=> v.Definition.ContainingType)));
                  
                    
                }
                
                
                1.ToString();
            }



            this.Projects = this.GetProjects().ToList().AsReadOnly();
        }

          
        private IList<Project> GetProjects()
        {
            var pattern = "Project\\(\"\\{[\\w-]*\\}\"\\) = \"([\\w _]*.*)\","+
                             " \"(.*\\.(cs|vcx|vb)proj)\"" +
                             ", \"({([a-zA-Z0-9_-]{36})})\"";
            // "Project\\(\"\\{[\\w-]*\\}\"\\) = \"([\\w _]*.*)\", \"(.*\\.(cs|vcx|vb)proj)\""
            Regex projReg = new Regex(pattern, RegexOptions.Compiled);
            var matches = projReg.Matches(Content).Cast<Match>().ToList();
          

            Func<Match, string> getIdFromMatch = (m) => 
                        Regex.Match(m.Value, "{.*?}").Value.Replace("{", "").Replace("}", "");

            var projectPaths = matches.Select(x => new
            {
                TypeId = getIdFromMatch(x),
                Path = x.Groups[2].Value,
                ID =    x.Groups[4].Value.Replace("{", "").Replace("}", "")
            } )
                                    .ToList();
            
          
            Func<string, string> getFullPath = (p) =>
               {

                   if (!System.IO.Path.IsPathRooted(p))
                       p = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(this.Path),
                          p);
                   p = System.IO.Path.GetFullPath(p);
                   return p;
               };
            return projectPaths.Select(p => new Project(getFullPath(p.Path), p.ID)).ToList();
        }
    }
}