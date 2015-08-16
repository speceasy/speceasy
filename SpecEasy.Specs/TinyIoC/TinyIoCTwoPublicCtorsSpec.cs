using NUnit.Framework;

namespace SpecEasy.Specs.TinyIoC
{
    internal class TinyIoCTwoPublicCtorsSpec : Spec<TwoPublicCtors>
    {
        public void ConstructorSelection()
        {
            When("constructing the SUT instance", () => EnsureSUT());

            Given("the target type has two public ctors with different numbers of parameters").Verify(() =>
                Then("the ctor with the fewest parameters should be used to construct the SUT", () =>
                {
                    Assert.IsNotNull(SUT.Arg1);
                    Assert.IsNull(SUT.Arg2);
                }));
        }
    }
}