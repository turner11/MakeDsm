using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;

namespace MakeDsm
{
    internal partial class Solution
    {
        internal partial class Project
        {
            public string ProjPath { get; }
            public string Name { get { return System.IO.Path.GetFileNameWithoutExtension(this.ProjPath); } }

            public string Content { get; }
            public XmlDocument ContentAsXml { get; }
            public string OutPutFileName { get; }

            public readonly IReadOnlyCollection<Denpendency> Dependencies;
            public readonly Guid Guid;

            public Project(string path, string id)
            {
                this.ProjPath = path;
                this.Guid = new Guid(id);
                this.Content = File.ReadAllText(this.ProjPath);
                var xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(this.Content);
                this.ContentAsXml = xmlDoc;


                var asNodes = xmlDoc.ChildNodes[1].ChildNodes.OfType<XmlNode>().Where(n => n.Name == "PropertyGroup")
                    .SelectMany(n => n.ChildNodes.OfType<XmlNode>())
                    .Where(n => n.Name == "AssemblyName")
                    .ToList();
                XmlNode asNode = asNodes.First();

                this.OutPutFileName = asNode.InnerText;

                this.Dependencies = this.GetDependencies().ToList().AsReadOnly();

            }

            private IList<Denpendency> GetDependencies()
            {
                FileInfo outPutFile = GetOutputFile();

                

                //var mscorlib = PortableExecutableReference.CreateFromFile(outPutFile.FullName);
                ////var mscorlib = PortableExecutableReference.CreateFromAssembly(typeof(object).Assembly);
                //var ws = new AdhocWorkspace();
                ////Create new solution
                //var solId = SolutionId.CreateNewId();
                //var solutionInfo = SolutionInfo.Create(solId, VersionStamp.Create());
                ////Create new project
                //var project = ws.AddProject("Sample", "C#");
                //project = project.AddMetadataReference(mscorlib);
                ////Add project to workspace
                //ws.TryApplyChanges(project.Solution);
                //string text = @"
                //class C
                //{
                //    void M()
                //    {
                //        M();
                //        M();
                //    }
                //}";
                ////CSharpSyntaxTree.ParseText(text);
                //var sourceText = SourceText.From(text);
                ////Create new document
                //var doc = ws.AddDocument(project.Id, "NewDoc", sourceText);
                ////Get the semantic model
                //var model = doc.GetSemanticModelAsync().Result;
                ////Get the syntax node for the first invocation to M()
                //var methodInvocation = doc.GetSyntaxRootAsync().Result.DescendantNodes().OfType<InvocationExpressionSyntax>().First();
                //var methodSymbol = model.GetSymbolInfo(methodInvocation).Symbol;
                ////Finds all references to M()
                //var referencesToM = SymbolFinder.FindReferencesAsync(methodSymbol, doc.Project.Solution).Result;





                try
                {


                    var asm = Assembly.LoadFile(outPutFile.FullName);
                    var types = asm.GetTypes();
                    var closures = types.Where(t => t.FullName.Contains("c__DisplayClass")).ToList();
                    var anonymusTypes = types.Where(t => t.FullName.Contains("__AnonymousType")).ToList();
                    var microsoftType = types.Where(t=> IsMicrosoftType(t)).ToList();
                    var a = types.Except(closures).Except(anonymusTypes).Except(microsoftType).ToList();


                    foreach (Type t in a)

                    {
                        Console.WriteLine(" " + t.FullName);

                    }
                    asm.ToString();
                }
                catch (ReflectionTypeLoadException ex)
                {

                    StringBuilder sb = new StringBuilder();
                    foreach (Exception exSub in ex.LoaderExceptions)
                    {
                        sb.AppendLine(exSub.Message);
                        FileNotFoundException exFileNotFound = exSub as FileNotFoundException;
                        if (exFileNotFound != null)
                        {
                            if (!string.IsNullOrEmpty(exFileNotFound.FusionLog))
                            {
                                sb.AppendLine("Fusion Log:");
                                sb.AppendLine(exFileNotFound.FusionLog);
                            }
                        }
                        sb.AppendLine();
                    }
                    string errorMessage = sb.ToString();
                    //Display or log the error based on your application.
                    throw new InvalidOperationException(errorMessage, ex);
                }

                //MethodBase methodBase = typeof(TestClass).GetMethod("Test");
                //var instructions = MethodBodyReader.GetInstructions(methodBase);

                //foreach (Instruction instruction in instructions)
                //{
                //    MethodInfo methodInfo = instruction.Operand as MethodInfo;

                //    if (methodInfo != null)
                //    {
                //        Type type = methodInfo.DeclaringType;
                //        ParameterInfo[] parameters = methodInfo.GetParameters();

                //        Console.WriteLine("{0}.{1}({2});",
                //            type.FullName,
                //            methodInfo.Name,
                //            String.Join(", ", parameters.Select(p => p.ParameterType.FullName + " " + p.Name).ToArray())
                //        );
                //    }
                //}

                throw new NotImplementedException();
            }

            public static bool IsMicrosoftType(Type type)
            {
                if (type == null)
                    throw new ArgumentNullException("type");

                if (type.Assembly == null)
                    return false;

                object[] atts = type.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), true);
                if ((atts == null) || (atts.Length == 0))
                    return false;

                AssemblyCompanyAttribute aca = (AssemblyCompanyAttribute)atts[0];
                var res = aca.Company?.IndexOf("Microsoft Corporation", StringComparison.OrdinalIgnoreCase) ?? -1;
                return res >= 0;
            }


            private FileInfo GetOutputFile()
            {
                var Directory = new DirectoryInfo(System.IO.Path.GetDirectoryName(this.ProjPath));
                var binFolderPath = System.IO.Path.Combine(Directory.FullName, "bin", "Debug");
                var binDirectory = new DirectoryInfo(binFolderPath);

                var outputExtension = new string[] { "dll", "exe" };
                var matches = outputExtension.SelectMany(e => binDirectory.GetFiles($"{this.OutPutFileName}.{e}"))
                                             .ToList();
                var outPutFile = matches.First();
                return outPutFile;
            }

            public override string ToString()
            {
                return this.Name;
            }

            public static string PrintXML(string XML)
            {
                string Result = "";

                MemoryStream mStream = new MemoryStream();
                XmlTextWriter writer = new XmlTextWriter(mStream, Encoding.Unicode);
                XmlDocument document = new XmlDocument();

                try
                {
                    // Load the XmlDocument with the XML.
                    document.LoadXml(XML);

                    writer.Formatting = Formatting.Indented;

                    // Write the XML into a formatting XmlTextWriter
                    document.WriteContentTo(writer);
                    writer.Flush();
                    mStream.Flush();

                    // Have to rewind the MemoryStream in order to read
                    // its contents.
                    mStream.Position = 0;

                    // Read MemoryStream contents into a StreamReader.
                    StreamReader sReader = new StreamReader(mStream);

                    // Extract the text from the StreamReader.
                    string FormattedXML = sReader.ReadToEnd();

                    Result = FormattedXML;
                }
                catch (XmlException)
                {
                    throw;
                }

                mStream.Close();
                writer.Close();

                return Result;
            }



        }
    }
}