using System;
using NUnit.Framework;
using SpecEasy.Specs.SutLifetime.SupportingExamples;

namespace SpecEasy.Specs.SutLifetime
{
    internal sealed class DisposableSubscriberSpec : Spec<DisposableSubscriber>
    {
        public void UpdatedCount()
        {
            DisposableSubscriber originalSUT = null;

            When("the updateable is updated", () => Raise<IUpdateable>(updateable => updateable.Updated += null, Get<IUpdateable>(), EventArgs.Empty));

            Given("SUT is constructed automatically", () => EnsureSUT()).Verify(() =>
                Then("the updated count should be 1", () => Assert.AreEqual(1, SUT.UpdatedCount)).
                Then("SUT has not been disposed", () => Assert.IsFalse(SUT.Disposed)));

            Given("an SUT instance is constructed and assigned explicitly", () =>
            {
                originalSUT = SUT;
                SUT = new DisposableSubscriber(Get<IUpdateable>());
            }).Verify(() =>
                Then("the updated count on the original SUT instance should be 0", () => Assert.AreEqual(0, originalSUT.UpdatedCount)).
                Then("the original SUT instance has been disposed", () => Assert.IsTrue(originalSUT.Disposed)).
                Then("the updated count on the current SUT instance should be 1", () => Assert.AreEqual(1, SUT.UpdatedCount)).
                Then("the current SUT instance has not been disposed", () => Assert.IsFalse(SUT.Disposed)).
                Then("the resolved SUT instance is the current instance", () => Assert.IsTrue(ReferenceEquals(SUT, Get<DisposableSubscriber>()))));
        }
    }
}
