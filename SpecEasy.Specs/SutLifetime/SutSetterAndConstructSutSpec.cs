using NUnit.Framework;
using Shouldly;
using SpecEasy.Specs.SutLifetime.SupportingExamples;

namespace SpecEasy.Specs.SutLifetime
{
    internal sealed class SutSetterAndConstructSutSpec : Spec<SutWithValueTypeDependency>
    {
        protected override SutWithValueTypeDependency ConstructSUT()
        {
            return new SutWithValueTypeDependency(456);
        }

        public void Run()
        {
            var value = 0;

            When("getting value from SUT", () => value = SUT.Value);

            Given($"{nameof(ConstructSUT)} has been overriden to directly construct an instance").Verify(() =>
                Then($"it should get the value used in {nameof(ConstructSUT)}", () => value.ShouldBe(456)).
                Then("the resolved instance is the same instance as the SUT property", () => Assert.IsTrue(ReferenceEquals(SUT, Get<SutWithValueTypeDependency>()))));

            Given("SUT has been explicitly set", () => SUT = new SutWithValueTypeDependency(123)).Verify(() =>
                Then("it should get the value provided when the SUT was explicitly set", () => value.ShouldBe(123)).
                Then("the resolved instance is the same instance as the SUT property", () => Assert.IsTrue(ReferenceEquals(SUT, Get<SutWithValueTypeDependency>()))));
        }
    }
}