using System;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;

namespace SpecEasy
{
    public class Spec<TUnit> : Spec
    {
        internal TinyIoCContainer MockingContainer;

        protected T Mock<T>() where T : class
        {
            return MockRepository.GenerateMock<T>();
        }

        protected T Get<T>()
        {
            return (T)MockingContainer.Resolve(typeof(T), new ResolveOptions
                                                              {
                                                                  UnregisteredResolutionRegistrationAction = (type, o) => MockingContainer.Register(type, o)
                                                              });
            //Preferred, but only allows reference types: return MockingContainer.Resolve<T>();
        }

        protected void Set<T>(T item)
        {
            MockingContainer.Register(typeof(T), item);
        }

        protected void Raise<T>(Action<T> eventSubscription, params object[] args) where T : class
        {
            var mock = Get<T>();
            mock.Raise(eventSubscription, args);
        }

        protected void AssertWasCalled<T>(Action<T> action)
        {
            var mock = Get<T>();
            mock.AssertWasCalled(action);
        }

        protected void AssertWasCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            var mock = Get<T>();
            mock.AssertWasCalled(action, methodOptions);
        }

        protected void AssertWasNotCalled<T>(Action<T> action)
        {
            var mock = Get<T>();
            mock.AssertWasNotCalled(action);
        }

        protected void AssertWasNotCalled<T>(Action<T> action, Action<IMethodOptions<object>> methodOptions)
        {
            var mock = Get<T>();
            mock.AssertWasNotCalled(action, methodOptions);
        }

        protected override void InitializeTest()
        {
            base.InitializeTest();
            MockingContainer = new TinyIoCContainer
                                   {
                                       FallbackRegistrationProvider = new AutoMockingRegistrationProvider()
                                   };
            MockingContainer.Register(typeof(TUnit)).AsSingleton();
        }

        protected TUnit SUT
        {
            get { return Get<TUnit>(); }
            set { Set(value); }
        }
    }

    internal class AutoMockingRegistrationProvider : IFallbackRegistrationProvider
    {
        public bool TryRegister(Type registerType, TinyIoCContainer container)
        {
            var mock = TryMockType(registerType);
            if (mock == null) return false;
            container.Register(registerType, mock);
            return true;
        }

        private static object TryMockType(Type type)
        {
            return type.IsInterface || type.IsAbstract ? MockRepository.GenerateMock(type, new Type[0]) : null;
        }
    }
}