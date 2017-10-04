using NUnit.Framework;
using SpecEasy.Specs.Disposal.SupportingExamples;

namespace SpecEasy.Specs.Disposal
{
    public sealed class DisposalSpec : Spec<DisposableCounter>
    {
        public void DisposeCount()
        {
            When("constructing SUT instances", () => { });

            Then("the instances are not disposed", () => Assert.AreEqual(0, SUT.DisposeCount));

            Given("an instance is explicitely disposed by the test", () => SUT.Dispose()).Verify(() =>
                Then("that instance is disposed a single time", () => Assert.AreEqual(1, SUT.DisposeCount)));

            Given("a first instance").Verify(() =>
                Then("the first instance is not disposed", () => Assert.AreEqual(0, SUT.DisposeCount)));

            Given("a second instance").Verify(() =>
                Then("the second instance is not disposed", () => Assert.AreEqual(0, SUT.DisposeCount)));
        }
    }
}
