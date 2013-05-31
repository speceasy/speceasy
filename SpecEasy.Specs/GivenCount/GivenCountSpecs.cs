using System.Collections.Generic;
using NUnit.Framework;
using SpecEasy.Specs.SetUpAndTearDownSpecs;

namespace SpecEasy.Specs.GivenCount
{
    public class GivenCountSpecs : Spec<FakeClass>
    {
        public void GivensAreCalledOncePerThenInTheProperOrderSpec()
        {
            var givenCalls = new Stack<int>();

            When("running a test that has nested givens preceding a then", () => SUT.DoNothing());

            Given("a given is called at nesting level 0.", () => givenCalls.Push(0)).Verify(() =>
                Given("a given is called at nesting level 1", () => givenCalls.Push(1)).Verify(() =>
                    Given("a given is called at nesting level 2", () => givenCalls.Push(2)).Verify(() =>
                            Then("each given should have been called only once and in the proper order", () =>
                                {
                                    Assert.That(givenCalls.Count, Is.EqualTo(3));
                                    Assert.That(givenCalls.Pop(), Is.EqualTo(2));
                                    Assert.That(givenCalls.Pop(), Is.EqualTo(1));
                                    Assert.That(givenCalls.Pop(), Is.EqualTo(0));
                                }))));
        }
    }
}
