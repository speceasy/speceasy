using NSubstitute;
using FluentAssertions;

namespace SpecEasy.Specs.GenericSpec
{
    public class AutoMockSpec : Spec<Mockable>
    {
        private const string StubbedDepValue = "stub-value-1";
        private const string ManuallyRegisteredDepValue = "dep-1-impl";

        public void Run()
        {
            When("testing a class with constructor dependencies", () => { });

            Given("an auto-mocked dependency").Verify(() =>
            {
                Then("it constructs SUT exactly once", () =>
                {
                    var sut1 = SUT;
                    var sut2 = SUT;
                    var sut3 = Get<Mockable>();
                    var sut4 = Get<Mockable>();
                    sut1.Should().BeSameAs(sut2);
                    sut2.Should().BeSameAs(sut3);
                    sut3.Should().BeSameAs(sut4);
                });

                Then("it gets a default mock object for dependency 1", () => SUT.Dep1.Should().NotBeOfType<Dependency1Impl>());

                Given("stubbed values for dependencies", () => Get<IDependency1>().Value.Returns(StubbedDepValue)).Verify(() =>
                    Then("it should produce the same instance of a mock dependency each time it's requested", () =>
                        Get<IDependency1>().Should().BeSameAs(Get<IDependency1>())).
                    Then("it should get stubbed values from its mocked dependencies", () =>
                        SUT.Dep1.Value.Should().Be(StubbedDepValue))
                );
            });

            Given("a dependency registered by the caller", () => Set<IDependency1>(new Dependency1Impl(ManuallyRegisteredDepValue))).Verify(() =>
                Then("it gets the same object each time a registered class is requested", () =>
                    Get<IDependency1>().Should().BeSameAs(Get<IDependency1>())).
                Then("it gets the same object each time an unregistered concrete object is requested", () =>
                    Get<Dependency2Impl>().Should().BeSameAs(Get<Dependency2Impl>())).
                Then("it gets the specified object for dependency 1", () =>
                    SUT.Dep1.Value.Should().Be(ManuallyRegisteredDepValue)));
        }
    }

    public class Mockable
    {
        public IDependency1 Dep1 { get; private set; }

        public Mockable(IDependency1 dep1)
        {
            Dep1 = dep1;
        }
    }

    public interface IDependency1
    {
        string Value { get; }
    }

    public class Dependency1Impl : IDependency1
    {
        public Dependency1Impl(string value)
        {
            Value = value;
        }

        public string Value { get; private set; }
    }

    public class Dependency2Impl
    {
        public Dependency2Impl()
        {
            Value = "dep-2-impl";
        }

        public string Value { get; private set; }
    }
}
