using NSubstitute;
using FluentAssertions;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public class GetSetInBeforeEachExampleSpec : Spec<GetSetInBeforeEachExampleSpec.ClassWithDependencies>
    {
        protected override void BeforeEachExample()
        {
            Get<IDependency>().GetValue().Returns(123);
            Set<IAnotherDependency>(new AnotherDependency(789));
        }

        public void Run()
        {
            var result = 0;

            When("using a dependency", () => result = SUT.UseDependency());

            Given("dependencies were set up in BeforeEachExample").Verify(() =>
                Then("it should get a value from the first dependency that was set up", () => result.Should().Be(123)).
                Then("it should get a value from the second dependency that was set up", () => SUT.UseAnotherDependency().Should().Be(789)));

            Given("dependency overridden in a Given", () =>
            {
                Get<IDependency>().GetValue().Returns(456);
                Set<IAnotherDependency>(new AnotherDependency(987));
            }).Verify(() =>
                Then("it should get the new value from the first dependency", () => result.Should().Be(456)).
                Then("it should get the new value from the second dependency", () => SUT.UseAnotherDependency().Should().Be(987)));
        }

        public class ClassWithDependencies
        {
            private readonly IDependency dependency;
            private readonly IAnotherDependency anotherDependency;

            public ClassWithDependencies(IDependency dependency, IAnotherDependency anotherDependency)
            {
                this.dependency = dependency;
                this.anotherDependency = anotherDependency;
            }

            public int UseDependency()
            {
                var value = dependency.GetValue();
                return value;
            }

            public int UseAnotherDependency()
            {
                return anotherDependency.GetAnotherValue();
            }
        }

        public interface IDependency
        {
            int GetValue();
        }

        public interface IAnotherDependency
        {
            int GetAnotherValue();
        }

        public class AnotherDependency : IAnotherDependency
        {
            private readonly int value;

            public AnotherDependency(int value)
            {
                this.value = value;
            }

            public int GetAnotherValue()
            {
                return value;
            }
        }
    }
}
