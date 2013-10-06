using System;
using Rhino.Mocks.Interfaces;
using SpecEasy.Core;

namespace SpecEasy.MockAdapters.RhinoMocks
{
    public interface IRhinoMockingFramework : IMockingFramework
    {
        void AssertWasCalled<T>(T mock, Action<T> action, Action<IMethodOptions<object>> methodOptions);
        void AssertWasNotCalled<T>(T mock, Action<T> action, Action<IMethodOptions<object>> methodOptions);
    }
}
