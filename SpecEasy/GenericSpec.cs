using System;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using TinyIoC;

namespace SpecEasy
{
    public class Spec<TUnit> : Spec
    {
        internal TinyIoCContainer MockingContainer;

        protected T Mock<T>() where T : class
        {
            return MockRepository.GenerateMock<T>();
        }

        private void RequireMockingContainer()
        {
            if (MockingContainer == null)
                throw new InvalidOperationException(
                    "This method cannot be called before the test context is initialized.");
        }

        protected T Get<T>()
        {
            RequireMockingContainer();
            return (T)MockingContainer.Resolve(typeof(T), new ResolveOptions
            {
                UnregisteredResolutionRegistrationOption = UnregisteredResolutionRegistrationOptions.RegisterAsSingleton,
                FallbackResolutionAction = TryAutoMock
            });
            //Preferred, but only allows reference types: return MockingContainer.Resolve<T>();
        }

        private object TryAutoMock(TinyIoCContainer.TypeRegistration registration, TinyIoCContainer container)
        {
            var type = registration.Type;
            return type.IsInterface || type.IsAbstract ? MockRepository.GenerateMock(type, new Type[0]) : null;
        }

        protected void Set<T>(T item)
        {
            RequireMockingContainer();
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

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
            MockingContainer = new TinyIoCContainer();
            MockingContainer.Register(typeof(TUnit)).AsSingleton();
        }

        protected void EnsureSUT()
        {
            Get<TUnit>();
        }

        protected TUnit SUT
        {
            get { return Get<TUnit>(); }
            set { Set(value); }
        }
    }
}