using NUnit.Framework;

namespace SpecEasy.Specs.TinyIoC
{
    internal class TinyIoCPublicAndPrivateCtorsSpec : Spec<PublicAndPrivateCtors>
    {
        public void ConstructorSelection()
        {
            When("construcing the SUT instance", () => EnsureSUT());

            Given("the type being constructed has both a public and private ctor and the private ctor is the one with the fewest parameters").Verify(() =>
                Then("the public ctor should be used to construct the SUT instance", () =>
                {
                    Assert.IsNotNull(SUT.Arg1);
                    Assert.IsNotNull(SUT.Arg2);
                }));
        }
    }
}