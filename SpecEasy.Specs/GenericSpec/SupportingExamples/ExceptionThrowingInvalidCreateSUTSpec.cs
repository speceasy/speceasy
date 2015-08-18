using System;
using NUnit.Framework;

namespace SpecEasy.Specs.GenericSpec.SupportingExamples
{
    [SupportingExample]
    internal sealed class ExceptionThrowingInvalidCreateSUTSpec : Spec<object>
    {
        internal static readonly Exception ConstructSUTThrownException = new Exception("manually constructing the SUT throws");

        public void RunSpec()
        {
            When("constructing SUT", () => EnsureSUT());

            Given("the SUT is constructed manually").Verify(() =>
                Given("the method to construct the SUT throws an exception").Verify(() =>
                    Then("the exception is thrown", () => Assert.Pass("We don't actually expect to hit this and pass the test; the assertion of the test runner will be that this test fails."))));
        }

        protected override object ConstructSUT()
        {
            throw ConstructSUTThrownException;
        }
    }
}