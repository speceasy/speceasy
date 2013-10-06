using System;
using TinyIoC;

namespace SpecEasy.Core
{
    public abstract class Spec<TUnit> : Spec
    {
        internal TinyIoCContainer MockingContainer;
        protected IMockingFramework MockingFramework;

        protected T Mock<T>() where T : class
        {
            return MockingFramework.GenerateMock<T>();
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
            return type.IsInterface || type.IsAbstract ? MockingFramework.GenerateMock(type, new Type[0]) : null;
        }

        protected void Set<T>(T item)
        {
            RequireMockingContainer();
            MockingContainer.Register(typeof(T), item);
        }

        protected void Raise<T>(Action<T> eventSubscription, params object[] args) where T : class
        {
            var mock = Get<T>();
            MockingFramework.Raise(mock, eventSubscription, args);
        }

        protected void AssertWasCalled<T>(Action<T> action)
        {
            var mock = Get<T>();
            MockingFramework.AssertWasCalled(mock, action);
        }

        protected void AssertWasNotCalled<T>(Action<T> action)
        {
            var mock = Get<T>();
            MockingFramework.AssertWasNotCalled(mock, action);
        }

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
            MockingContainer = new TinyIoCContainer();
            MockingContainer.Register(typeof(TUnit)).AsSingleton();
            MockingFramework = GetConcreteMockingFramework();
        }

        protected TUnit SUT
        {
            get { return Get<TUnit>(); }
            set { Set(value); }
        }

        protected abstract IMockingFramework GetConcreteMockingFramework();
    }
}