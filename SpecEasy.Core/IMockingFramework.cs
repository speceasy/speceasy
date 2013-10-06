using System;

namespace SpecEasy.Core
{
    public interface IMockingFramework
    {
        T GenerateMock<T>(params object[] argumentsForConstructor) where T : class;
        object GenerateMock(Type type, Type[] extraTypes, params object[] argumentsForConstructor);

        void Raise<TEventSource>(TEventSource mockObject, Action<TEventSource> eventSubscription,
                                 params object[] args) where TEventSource : class;

        void AssertWasCalled<T>(T mock, Action<T> action);

        void AssertWasNotCalled<T>(T mock, Action<T> action);
    }
}
