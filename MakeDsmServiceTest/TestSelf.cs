using MakeDsm;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MakeDsmServiceTest
{
    [TestFixture]
    public class TestSelf
    {
        [Test]
        public void TestMethod()
        {
            var currAssemblyPath = Assembly.GetExecutingAssembly().Location;
            var currDir = new DirectoryInfo( Path.GetDirectoryName(currAssemblyPath));
            var solPath = currDir.Parent.Parent.Parent;//System.IO.Path.Combine(currPath, "\\..");
            var makePath = solPath.GetFiles("*.sln").First().FullName;
            var results = MakeDsmService.GetDependencies(makePath);
            var dic = results.DependencyDictionary;

            Assert.Contains("factorial.cpp", dic["factorial.cpp"]?.ToArray());
            Assert.Contains("hello.cpp", dic["hello.cpp"]?.ToArray());
            Assert.Contains("main.cpp", dic["main.cpp"]?.ToArray());
        }
    }
}
