using MakeDsm;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsmServiceTest
{
    [TestFixture]
    public class TestClass
    {
        [Test]
        public void TestFactorialDependencies()
        {

            var makeText = Properties.Resources.m;

            var path = this.WriteFactorialFile();
            var results = MakeDsmService.GetDependencies(path, makeText);

            
            Assert.Contains("factorial.cpp", results["factorial.cpp"]?.ToArray());
            Assert.Contains("hello.cpp", results["hello.cpp"]?.ToArray());
            Assert.Contains("main.cpp", results["main.cpp"]?.ToArray());       
        }

        private string WriteFactorialFile()
        {
            var path = Path.Combine(Path.GetTempPath(),"factorialTest");
            Directory.CreateDirectory(path);

            var fFactorial = Properties.Resources.factorial;//factorial.cpp
            File.WriteAllText(Path.Combine(path, "factorial.cpp"), fFactorial);

            var fHello = Properties.Resources.hello;//hello.cpp
            File.WriteAllText(Path.Combine(path, "hello.cpp"), fHello);

            var fMain = Properties.Resources.main;//main.cpp
            File.WriteAllText(Path.Combine(path, "main.cpp"), fMain);

            var fFunctions = Properties.Resources.functions;//functions.h
            File.WriteAllText(Path.Combine(path, "functions.h"), fFunctions);

            return path;
        }
    }
}
