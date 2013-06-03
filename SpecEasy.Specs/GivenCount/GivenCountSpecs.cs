using System.Collections.Generic;
using NUnit.Framework;
using SpecEasy.Specs.SetUpAndTearDownSpecs;

namespace SpecEasy.Specs.GivenCount
{
    public class GivenCountSpecs : Spec<FakeClass>
    {
        private List<string> _givenCalls;

        public void GivensAreCalledOncePerThenInTheProperOrderWithSimpleNestingSpec()
        {
            _givenCalls = new List<string>();
            When("running a test that has nested givens preceding a then", () => SUT.DoNothing());

            Given("a given is called at nesting level 0.", () => _givenCalls.Add("0")).Verify(() =>
                Given("a given is called at nesting level 1", () => _givenCalls.Add("1")).Verify(() =>
                    Given("a given is called at nesting level 2", () => _givenCalls.Add("2")).Verify(() =>
                            Then("each given should have been called only once and in the proper order", () => 
                                VerifyGivenCalls("0 -> 1 -> 2", clearValuesAfterVerify: false)).
                           Then("each given is called a second time for the second then", () => 
                               VerifyGivenCalls("0 -> 1 -> 2 -> 0 -> 1 -> 2", clearValuesAfterVerify: false)))));
        }

        public void GivensAreCalledOncePerThenInTheProperOrderWithMixedNestingSpec()
        {
            _givenCalls = new List<string>();

            When("running a test that has nested givens at varying levels with then calls intertwined.", () => SUT.DoNothing());

            Given("a top level given is used to establish 'global' context for the spec", () => _givenCalls.Add("0")).Verify(() =>
                {
                    Given("a nested given is used at level 1.1", () => _givenCalls.Add("1.1")).Verify(() =>
                        Given("a nested given is used at level 2.1", () => _givenCalls.Add("2.1")).Verify(() =>
                        Then("each given should be called once leading up to this then", () =>
                            VerifyGivenCalls("0 -> 1.1 -> 2.1"))));

                    Given("a nested given is used at level 1.2", () => _givenCalls.Add("1.2")).Verify(() =>
                        {
                            Given("a nested given is used at level 2.2", () => _givenCalls.Add("2.2")).Verify(() =>
                                Then("each given leading up to this then should be called once.", () =>
                                    VerifyGivenCalls("0 -> 1.2 -> 2.2")));

                            Given("a nested given is used at level 2.3", () => _givenCalls.Add("2.3")).Verify(() =>
                                Given("a nested given is used at level 3.1", () => _givenCalls.Add("3.1")).Verify(() =>
                                    {
                                        Then("all givens leading up to this then should be called once in the proper order.", () =>
                                            VerifyGivenCalls("0 -> 1.2 -> 2.3 -> 3.1", clearValuesAfterVerify: true));

                                        Then("a second then immediately following the first should repeat the same givens in the same order.", () =>
                                            VerifyGivenCalls("0 -> 1.2 -> 2.3 -> 3.1"));
                                    }));
                        });
                });
        }

        private void VerifyGivenCalls(string expectedValue, bool clearValuesAfterVerify = true)
        {
            var givenCallListString = string.Join(" -> ", _givenCalls);
            Assert.That(givenCallListString, Is.EqualTo(expectedValue));
            if (clearValuesAfterVerify) _givenCalls.Clear();
        }
    }
}
