using System;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace SpecEasy.MockAdapters.RhinoMocks
{
    public class MockingFramework : IRhinoMockingFramework
    {
        public T GenerateMock<T>(params object[] argumentsForConstructor) where T : class
        {
            return MockRepository.GenerateMock<T>();
        }

        public object GenerateMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor)
        {
            return MockRepository.GenerateMock(type, extraTypes, argumentsForConstructor);
        }

        public void Raise<TEventSource>(TEventSource mockObject, Action<TEventSource> eventSubscription, params object[] args) where TEventSource : class
        {
            mockObject.Raise(eventSubscription, args);
        }

        public void AssertWasCalled<T>(T mock, Action<T> action)
        {
            mock.AssertWasCalled(action);
        }

        public void AssertWasCalled<T>(T mock, Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            mock.AssertWasCalled(action, methodOptions);
        }

        public void AssertWasNotCalled<T>(T mock, Action<T> action)
        {
            mock.AssertWasNotCalled(action);
        }

        public void AssertWasNotCalled<T>(T mock, Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            mock.AssertWasNotCalled(action, methodOptions);
        }
    }
}
