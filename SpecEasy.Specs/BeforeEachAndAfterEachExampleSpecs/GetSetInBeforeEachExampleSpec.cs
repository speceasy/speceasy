using NUnit.Framework;
using Rhino.Mocks;

namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    public class GetSetInBeforeEachExampleSpec : Spec<GetSetInBeforeEachExampleSpec.ClassWithDependencies>
    {
        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
            Get<IDependency>().Stub(dep => dep.GetValue()).Return(123);
            Set<IAnotherDependency>(new AnotherDependency(789));
        }

        public void Run()
        {
            var result = 0;

            When("using a dependency", () => result = SUT.UseDependency());
            
            Given("dependencies were set up in BeforeEachExample").Verify(() =>
                Then("it should get a value from the first dependency that was set up", () => Assert.That(result, Is.EqualTo(123))).
                Then("it should get a value from the second dependency that was set up", () => Assert.That(SUT.UseAnotherDependency(), Is.EqualTo(789))));

            Given("dependency overriden in a Given", () =>
            {
                Get<IDependency>().Stub(dep => dep.GetValue()).Return(456).Repeat.Any(); // Repeat.Any replaces the previous Stub
                Set<IAnotherDependency>(new AnotherDependency(987));
            }).Verify(() =>
                Then("it should get the new value from the first dependency", () => Assert.That(result, Is.EqualTo(456))).
                Then("it should get the new value from the second dependency", () => Assert.That(SUT.UseAnotherDependency(), Is.EqualTo(987))));
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
