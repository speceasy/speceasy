using NUnit.Framework;
using Rhino.Mocks;

namespace SpecEasy.Specs.GenericSpec
{
    public class AutoMockSpec : Spec<Mockable>
    {
        public void Run()
        {
            When("testing a class with constructor dependencies", () => { });

            Given("an auto-mocked dependency").Verify(() =>
            {
                Then("it constructs SUT exactly once", () =>
                {
                    var sut1 = SUT;
                    var sut2 = SUT;
                    Assert.That(sut1, Is.SameAs(sut2));
                });
                Then("it gets a default mock object for dependency 1", () => Assert.That(SUT.Dep1, Is.Not.InstanceOf<Dependency1Impl>()));
                Given("stubbed values for depedencies", () => Get<IDependency1>().Stub(d => d.Value).Return("stub-value-1")).Verify(() =>
                    Then("it should produce the same instance of a mock dependency each time it's requested", () =>
                        Assert.That(Get<IDependency1>(), Is.SameAs(Get<IDependency1>()))).
                    Then("it should get stubbed values from its mocked dependencies", () =>
                        Assert.That(SUT.Dep1.Value, Is.EqualTo("stub-value-1")))
                );
            });

            Given("a dependency registered by the caller", () => Set<IDependency1>(new Dependency1Impl("dep-1-impl"))).Verify(() =>
                Then("it gets the same object each time a registered class is requested", () =>
                {
                    Assert.That(Get<IDependency1>(), Is.SameAs(Get<IDependency1>()));
                }).
                Then("it gets the same object each time an unregistered concrete object is requested", () =>
                {
                    Assert.That(Get<Dependency2Impl>(), Is.SameAs(Get<Dependency2Impl>()));
                }).
                Then("it gets the specified object for dependency 1", () => Assert.That(SUT.Dep1.Value, Is.EqualTo("dep-1-impl"))));
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
