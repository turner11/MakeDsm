using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsm
{
    public abstract class MakeDsmService
    {
        public string MakeFilePath { get; }
        public string CodePath { get; }
        public virtual string Text { get; }

        public static Denpendencies GetDependencies(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException($"File '{path}' does not exist.");

            MakeDsmService service;
            var ext = System.IO.Path.GetExtension(path).ToLower();
            switch (ext)
            {
                case ".sln":
                    service = new MakeDsm_CS(path);
                    break;

                case ".mk":
                    service = new MakeDsm_C(path);
                    break;
                default:
                    throw new ArgumentException($"Cannot build dependencies for unknown type '{ext}'", nameof(path));
                    
            }
            
            return service.GetDependencies();
        }


        protected MakeDsmService(string path)
        {
            if (!File.Exists(path))
                throw new ArgumentException($"File '{path}' does not exist.");
            this.MakeFilePath = path;
            this.CodePath = Path.GetDirectoryName(path);
            this.Text = File.ReadAllText(path);
        }

        protected abstract Denpendencies GetDependencies();
    

        internal class Module
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
