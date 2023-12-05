using FluentAssertions;
using SpecEasy.Specs.SutLifetime.SupportingExamples;

namespace SpecEasy.Specs.SutLifetime
{
    internal sealed class SutInBeforeEachExampleSpec : Spec<SutWithValueTypeDependency>
    {
        protected override void BeforeEachExample()
        {
            SUT = new SutWithValueTypeDependency(123);
        }

        public void Run()
        {
            var value = 0;

            When("getting the value from SUT", () => value = SUT.Value);

            Given($"SUT was set up in {nameof(BeforeEachExample)}").Verify(() =>
                Then($"it should get the value from the SUT set up in {nameof(BeforeEachExample)}", () => value.Should().Be(123)));
        }
    }
}
