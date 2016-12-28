using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MakeDsm.MakeDsmService;

namespace MakeDsm
{
    internal class MakeDsm_C: MakeDsmService
    {
        const string TEST_MAKE_FILE_NAME = "TestMakeFile.mk";
        const string NMAKE_PATH = @"C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\bin\nmake.exe";

        public MakeDsm_C(string path) : base(path)
        {
        }

   

        private static Dictionary<string, List<string>> GetDependencies(string path, string makeFileText)
        {
            List<string> moduls = GetAllModuleNames(makeFileText);
            var dependencies = AnalyzeModules(moduls, path);
            return dependencies;
        }

        private static Dictionary<string, List<string>> AnalyzeModules(List<string> moduleNames, string path)
        {

            var modules = moduleNames.Select(m => new Module(m, new List<string> { m })) //each module depends at least on itseelf 
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
                moduleNames.ToDictionary(mn => mn,
                mn => (modules.Where(m => m.Name == mn).FirstOrDefault()?.Dependecies
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
            
            //no window
            info.UseShellExecute = false;
            info.CreateNoWindow = true;
            info.WindowStyle = ProcessWindowStyle.Hidden;
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

                 Debug.WriteLine("output>>" + (String.IsNullOrEmpty(output) ? "(none)" : output));
                 Debug.WriteLine("error>>" + (String.IsNullOrEmpty(error) ? "(none)" : error));
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
                var fn = System.IO.Path.Combine(path, TEST_MAKE_FILE_NAME);
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

        protected override Denpendencies GetDependencies()
        {
            var dic = MakeDsm_C.GetDependencies(this.CodePath, this.Text);
            return new Denpendencies(dic);
        }
    }
}
