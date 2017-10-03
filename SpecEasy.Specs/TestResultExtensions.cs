using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace SpecEasy.Specs
{
    public static class TestResultExtensions
    {
        public static IEnumerable<ITestResult> FailedTests(this ITestResult testResult)
        {
            return testResult.AllTests()
                .Where(result => result.ResultState.Status == TestStatus.Failed)
            ;
        }

        public static IEnumerable<ITestResult> AllTests(this ITestResult testResult)
        {
            if (!testResult.HasChildren)
            {
                return new[] { testResult };
            }

            return testResult.Children
                .SelectMany(childResult => childResult.AllTests())
                .Where(result => !(result is TestSuiteResult))
            ;
        }
    }
}