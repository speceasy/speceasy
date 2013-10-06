using System;
using Rhino.Mocks.Interfaces;
using SpecEasy.Core;
using SpecEasy.MockAdapters.RhinoMocks;

namespace SpecEasy
{
    public class Spec<TUnit> : Core.Spec<TUnit>
    {
        protected void AssertWasCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            var mock = Get<T>();
            ((IRhinoMockingFramework)MockingFramework).AssertWasCalled(mock, action, methodOptions);
        }

        protected void AssertWasNotCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            var mock = Get<T>();
            ((IRhinoMockingFramework)MockingFramework).AssertWasNotCalled(mock, action, methodOptions);
        }

        protected override IMockingFramework GetConcreteMockingFramework()
        {
            return new MockingFramework();
        }
    }
}