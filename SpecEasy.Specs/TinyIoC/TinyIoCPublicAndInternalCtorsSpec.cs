using NUnit.Framework;

namespace SpecEasy.Specs.TinyIoC
{
    internal class TinyIoCPublicAndInternalCtorsSpec : Spec<PublicAndInternalCtors>
    {
        public void ConstructorSelection()
        {
            When("constructing the SUT instance", () => EnsureSUT());

            Given("the type being constructed has both public and internal constructors and the internal constructor has the fewest parameters").Verify(() =>
                Then("the public ctor should be used to construct the SUT instance", () =>
                {
                    Assert.IsNotNull(SUT.Arg1);
                    Assert.IsNotNull(SUT.Arg2);
                }));
        }   
    }
}