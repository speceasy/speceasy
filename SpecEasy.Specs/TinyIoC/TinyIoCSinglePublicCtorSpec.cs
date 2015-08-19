using NUnit.Framework;

namespace SpecEasy.Specs.TinyIoC
{
    internal class TinyIoCSinglePublicCtorSpec : Spec<SinglePublicCtor>
    {
        public void ConstructorSelection()
        {
            When("constructing the SUT instance", () => EnsureSUT());

            Given("the target type has a single public ctor").Verify(() =>
                Then("the single public ctor should be used to construct the SUT", () => Assert.IsNotNull(SUT.Arg1)));
        }
    }
}