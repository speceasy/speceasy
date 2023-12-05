using System;
using NUnit.Framework;
using FluentAssertions;

namespace SpecEasy.Specs.GenericSpec
{
    public class ManualSUTConstructionSpec : Spec<Mockable>
    {
        private const string ManualConstructedDependencyValue = "Manually Constructed";

        private int constructSUTCallCount;
        private bool shouldManauallyConstructSUT;

        private Exception constructSUTThrownException;
        private bool constructSUTReturnsNull;

        public void ManuallyConstructSUT()
        {
            When("testing a class that may be constructed manually", () => { });

            Given("the SUT is never accessed").Verify(() =>
                Then("the method to construct the SUT is never called", () => constructSUTCallCount.Should().Be(0)));

            Given("the SUT is constructed automatically", () => shouldManauallyConstructSUT = false).Verify(() =>
            {
                Given("the SUT is accessed multiple times", () =>
                {
                    EnsureSUT();
                    EnsureSUT();
                }).Verify(() =>
                {
                    Then("the method to construct the SUT should only be called once", () => constructSUTCallCount.Should().Be(1));
                    Then("the same instance of the SUT is returned every time", () =>
                    {
                        var sut1 = SUT;
                        var sut2 = SUT;
                        var sut3 = Get<Mockable>();
                        var sut4 = Get<Mockable>();
                        sut1.Should().BeSameAs(sut2);
                        sut2.Should().BeSameAs(sut3);
                        sut3.Should().BeSameAs(sut4);
                    });
                });

                Then("the dependency should be the one automatically constructed", () => SUT.Dep1.Should().BeSameAs(Get<IDependency1>()));
            });

            Given("the SUT is constructed manually", () => shouldManauallyConstructSUT = true).Verify(() =>
            {
                Given("the SUT is accessed multiple times", () =>
                {
                    EnsureSUT();
                    EnsureSUT();
                }).Verify(() =>
                {
                    Then("the method to construct the SUT should only be called once", () => constructSUTCallCount.Should().Be(1));
                    Then("the same instance of the SUT is returned every time", () =>
                    {
                        var sut1 = SUT;
                        var sut2 = SUT;
                        var sut3 = Get<Mockable>();
                        var sut4 = Get<Mockable>();
                        sut1.Should().BeSameAs(sut2);
                        sut2.Should().BeSameAs(sut3);
                        sut3.Should().BeSameAs(sut4);
                    });
                });

                Given("the method to construct the SUT throws an exception", () => constructSUTThrownException = new Exception("manually constructing the SUT throws")).Verify(() =>
                    Given("the SUT is never accessed", () => {}).Verify(() =>
                        Then("no exception is thrown", () => Assert.Pass())));

                Given("the method to construct the SUT returns null", () => constructSUTReturnsNull = true).Verify(() =>
                    Given("the SUT is never accessed", () => {}).Verify(() =>
                        Then("no exception is thrown", () => Assert.Pass())));

                Then("the dependency should be the one manually constructed", () => SUT.Dep1.Value.Should().Be(ManualConstructedDependencyValue));
            });
        }

        protected override Mockable ConstructSUT()
        {
            constructSUTCallCount++;

            if (!shouldManauallyConstructSUT)
            {
                return base.ConstructSUT();
            }

            if (constructSUTThrownException != null)
            {
                throw constructSUTThrownException;
            }

            if (constructSUTReturnsNull)
            {
                return null;
            }

            return new Mockable(new Dependency1Impl(ManualConstructedDependencyValue));
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();

            constructSUTCallCount = 0;
            constructSUTThrownException = null;
            constructSUTReturnsNull = false;
        }
    }
}