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

        public static IEnumerable<TestResult> AllTests(this TestResult testResult)
        {
            if (testResult.Results == null || testResult.Results.Count == 0)
                return new[] { testResult };

            var tests = new List<TestResult>();
            foreach (var childResult in testResult.Results)
                tests.AddRange(AllTests((TestResult)childResult));
            return tests;
        }
    }
}