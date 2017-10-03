using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using NUnit.Framework.Internal.Builders;

namespace SpecEasy.Internal
{
    internal sealed class SpecTestCaseSourceAttribute : Attribute, ITestBuilder
    {
        public IEnumerable<TestMethod> BuildFrom(IMethodInfo method, Test suite)
        {
            if (!method.TypeInfo.Type.IsSubclassOf(typeof(Spec)))
            {
                throw new InvalidOperationException($"Only classes inheriting from {nameof(Spec)} can be marked with {nameof(SpecTestCaseSourceAttribute)}");
            }

            var instance = (Spec)method.TypeInfo.Construct(new object[0]);
            var builder = new NUnitTestCaseBuilder();
            return instance.TestCases.Select(data => builder.BuildTestMethod(method, suite, new TestCaseParameters(data)));
        }
    }
}