using System;
using Shouldly;

namespace SpecEasy.Specs.TinyIoC
{
    internal class TinyIoCSinglePrivateCtorSpec : Spec<SinglePrivateCtor>
    {
        public void ConstructorSelection()
        {
            When("constructing the SUT instance", () => EnsureSUT());

            Given("the type being constructed has a single private constructor").Verify(() =>
                Then("an exception should be thrown because we will not use private ctors to build the SUT instance", () => AssertWasThrown<Exception>(ex => ex.GetType().FullName.ShouldStartWith("TinyIoC.TinyIoCResolutionException"))));
        }
    }
}