using System;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Core.Filters;
using Should;
using SpecEasy;
using NUnit.Core;
using NUnit.Framework;

namespace Examples
{
    [TestFixture]
    public class ExceptionReportingSpecs
    {

        [Test]
        public void RunSpecsForBrokenClass()
        {
            CoreExtensions.Host.InitializeService();
            var pathToTestLibrary = Assembly.GetExecutingAssembly().Location;
            var testPackage = new TestPackage(pathToTestLibrary)
            {
                BasePath = Path.GetDirectoryName(pathToTestLibrary)
            };
            var builder = new TestSuiteBuilder();
            var suite = builder.Build(testPackage);
            var filter = new SimpleNameFilter("Examples.BrokenClassSpec");
            var result = suite.Run(new NullListener(), filter);

            var failedTests = result.FindFailedTests();
            Assert.IsTrue(failedTests.All(ft => ft.Message.StartsWith("System.Exception")), "At least one failed spec reported an assertion error when it should have reported an exception.");
        }

    }

    internal class BrokenClassSpec : Spec<BrokenClass>
    {
        private bool input;
        private bool result;

        public void RunSpec()
        {
            When("inverting a boolean value", () => result = SUT.Invert(input));

            Given("a false input", () => input = false).Verify(() =>
                Then("the result is true", () => result.ShouldBeTrue()));

            Given("a true input", () => input = true).Verify(() =>
                Then("the result is false", () => result.ShouldBeFalse()));
        }
    }

    public class BrokenClass
    {
        public bool Invert(bool value)
        {
            throw new Exception("exception while trying to invert value");
            return !value;
        }
    }

}