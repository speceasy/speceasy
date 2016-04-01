using System.Collections.Generic;
using NUnit.Core;

namespace SpecEasy.Specs
{
    public static class TestResultExtensions
    {
        public static IEnumerable<TestResult> FailedTests(this TestResult testResult)
        {
            if (testResult.FailureSite == FailureSite.Test)
                return new[] { testResult };

            var failedTests = new List<TestResult>();
            foreach (var childResult in testResult.Results)
                failedTests.AddRange(FailedTests((TestResult)childResult));
            return failedTests;
        }
    }
}