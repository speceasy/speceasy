using System.IO;
using System.Reflection;
using NUnit.Core;
using NUnit.Core.Filters;

namespace SpecEasy.Specs
{
    public static class SpecRunner
    {
        public static TestResult Run<T>() where T : Spec
        {
            CoreExtensions.Host.InitializeService();

            var pathToExecutingAssembly = Assembly.GetExecutingAssembly().Location;

            var testPackage = new TestPackage(pathToExecutingAssembly)
            {
                BasePath = Path.GetDirectoryName(pathToExecutingAssembly)
            };

            var builder = new TestSuiteBuilder();
            var suite = builder.Build(testPackage);
            var filter = new SimpleNameFilter(typeof(T).FullName);
            var result = suite.Run(new NullListener(), filter);

            return result;
        }
    }
}
