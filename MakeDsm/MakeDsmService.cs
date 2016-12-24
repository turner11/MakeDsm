using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm
{
    public static class MakeDsmService
    {

        const string TEST_MAKE_FILE_NAME = "TestMakeFile.mk";
        const string NMAKE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\nmake.exe";
        public static Dictionary<string, List<string>> GetDependencies(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException($"File '{path}' does not exist.");

           
            var txt = File.ReadAllText(path);
            return GetDependencies(path, txt);




        }

        public static Dictionary<string, List<string>> GetDependencies(string path, string makeFileText)
        {
            List<string> moduls = GetAllModuleNames(makeFileText);
            var dependencies = AnalyzeModules(moduls,path);
            return dependencies;
        }

        private static Dictionary<string, List<string>> AnalyzeModules(List<string> moduleNames, string path)
        {

            var modules = moduleNames.Select(m => new Module(m , new List<string> { m })) //each module depends at least on itseelf 
                                        .ToList();

            var resolvedModules = new List<Module>();
            var nonResolvedModules = modules.ToList();

            for (int i = 0; i < modules.Count && nonResolvedModules.Count > 0; i++)
            {
                var currModule = modules[i];

                var allCombinations = GetOrederedPowerSet(resolvedModules)
                                        .Select(c => c.ToList()).ToList();

                var modulesForTest = allCombinations
                    .Select(comb => comb.Union(new Module[] { currModule }).ToList());

                bool isBuilt = false;
                foreach (var comb in modulesForTest)
                {
                    string makeFileText = BuildMakeFile(comb);
                    CleanMake(path);
                    isBuilt = RunMake(path, makeFileText);
                    CleanMake(path);

                    if (isBuilt)
                    {
                        currModule.SetDependencies(comb);
                        break;
                    }

                }

                if (isBuilt)
                {
                    resolvedModules.Add(currModule);
                    nonResolvedModules.Remove(currModule);
                }

            }


            var dic = 
                moduleNames.ToDictionary(mn=> mn, 
                mn=> (modules.Where(m=> m.Name == mn).FirstOrDefault()?.Dependecies 
                      ?? new string[0])
                     .ToList());

            return dic;
        }

        private static string BuildMakeFile(List<Module> modules)
        {
            var sb = new StringBuilder();

            foreach (var rModule in modules)
            {

                var moduleBuild = rModule.GetBuildText();
                sb.AppendLine(moduleBuild);
                sb.AppendLine();
            }
            var cleanText =
@"clean:
    del *.o";
            sb.AppendLine(cleanText);

          
            var txt = sb.ToString();
            return txt;
        }

        static IEnumerable<IEnumerable<T>> GetPowerSet<T>(List<T> list)
        {
            return from m in Enumerable.Range(0, 1 << list.Count)
                   select
                       from i in Enumerable.Range(0, list.Count)
                       where (m & (1 << i)) != 0
                       select list[i];
        }


        static IOrderedEnumerable<IEnumerable<T>> GetOrederedPowerSet<T>(List<T> list)
        {
            return GetPowerSet(list).OrderBy(en => en.Count());
        }

        private static List<string> GetAllModuleNames(string makeFileText)
        {
            var lines = makeFileText.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            var moduleLines = lines.Where(l => l.Split(new string[] { " " }, StringSplitOptions.None).First().EndsWith(".o:", StringComparison.InvariantCultureIgnoreCase)).ToArray();
            var moduls = moduleLines.SelectMany(l => l.Split(new string[] { " " }, StringSplitOptions.None).Skip(1)).ToList();
            return moduls;
        }

        private static void CleanMake(string path)
        {
            //nmake /f m.mk clean
            string command = NMAKE_PATH;
            string args = $"/f {TEST_MAKE_FILE_NAME} clean";


            ProcessStartInfo info = new ProcessStartInfo();
            info.WorkingDirectory = path;
            info.FileName = command;
            info.Arguments = args;
            //info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.RedirectStandardOutput = true;
            info.RedirectStandardError = true;

            bool success;
            using (Process process = Process.Start(info))
            {
                process.WaitForExit();

                // *** Read the streams ***
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                var exitCode = process.ExitCode;

                // Debug.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                // Debug.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                //Debug.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                process.Close();
                success = exitCode == 0;
                // do something with your process. If you're capturing standard output,
                // you'll also need to capture standard error. Be careful to avoid the
                // deadlock bug mentioned in the docs for
                // ProcessStartInfo.RedirectStandardOutput. 
            }

        }

        private static bool RunMake(string path, string makeFileText)
        {
            bool success;
            try
            {
                var fn = Path.Combine(path, TEST_MAKE_FILE_NAME);
                File.WriteAllText(fn, makeFileText);


                string command = NMAKE_PATH;
                string args = TEST_MAKE_FILE_NAME;


                ProcessStartInfo info = new ProcessStartInfo();
                info.WorkingDirectory = path;
                info.FileName = command;
                info.Arguments = args;
                //info.CreateNoWindow = true;
                info.UseShellExecute = false;
                info.RedirectStandardOutput = true;
                info.RedirectStandardError = true;


                using (Process process = Process.Start(info))
                {
                    process.WaitForExit();

                    // *** Read the streams ***
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    var exitCode = process.ExitCode;

                    // Debug.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                    // Debug.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
                    //Debug.WriteLine("ExitCode: " + exitCode.ToString(), "ExecuteCommand");
                    process.Close();
                    success = exitCode == 0;
                    // do something with your process. If you're capturing standard output,
                    // you'll also need to capture standard error. Be careful to avoid the
                    // deadlock bug mentioned in the docs for
                    // ProcessStartInfo.RedirectStandardOutput. 
                }
           
            }
            catch (Exception ex)
            {
                throw;
                //success = false;
            }
            return success;
        }


       
        class Module
        {
            public string Name { get; }
            List<string> _dependecies {get; set;}
            public IReadOnlyCollection<string> Dependecies
            {
                get
                {
                    return this._dependecies.Distinct().ToList().AsReadOnly();
                }
            }
            public Module(string name, IList<string> dependencies)
            {
                this.Name = name;
                this._dependecies = new List<string>(dependencies);
            }


            public override string ToString()
            {
                return $"{ this.Name}:  {String.Join(" ",this._dependecies)}";
            }

            public override bool Equals(object obj)
            {
                var other = obj as Module;
                if (other == null)
                    return false;
                return this.Name.Equals(other.Name,StringComparison.InvariantCultureIgnoreCase);
            }

            public override int GetHashCode()
            {
                return this.Name.ToUpper().GetHashCode();
            }

        
            internal string GetBuildText()
            { 
                var objName = this.Name.Replace(".cpp", ".o");
                var depText = String.Join(" ", this.Dependecies.Select(d => d));
                var header = $"{objName}: {depText}";
                var buildLine = $"    g++ -c {depText}";
                var buildText  = header    +Environment.NewLine;
                    buildText += buildLine;
                return buildText;
            }

            internal void SetDependencies(List<Module> modules)
            {
                this._dependecies = modules.SelectMany(m => m.Dependecies).Distinct().ToList();
            }
        }
    }
}
