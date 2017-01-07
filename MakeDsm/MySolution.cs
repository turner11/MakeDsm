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
    internal partial class MySolution
    {
        public readonly IReadOnlyCollection<Project> Projects;
        public string Path { get; }
        private readonly string Content;

        public MySolution(string slnPath)
        {
            this.Path = slnPath;
            this.Content = System.IO.File.ReadAllText(slnPath);
          
            string solutionPath = slnPath;
            
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