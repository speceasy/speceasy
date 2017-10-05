using System;
using System.Reflection;
using Rhino.Mocks;
using Rhino.Mocks.Interfaces;
using TinyIoC;

namespace SpecEasy
{
    public class Spec<TUnit> : Spec
    {
        private TinyIoCContainer mockingContainer;
        private ResolveOptions resolveOptions;
        private bool alreadyConstructedSUT;
        private TUnit constructedSUTInstance;

        protected TUnit SUT
        {
            get { return GetSUTInstance(); }
            set
            {
                if (!typeof(TUnit).IsValueType && ReferenceEquals(constructedSUTInstance, value))
                {
                    return;
                }

                constructedSUTInstance = value;
                Set(value);
                alreadyConstructedSUT = true;
            }
        }

        private ResolveOptions ResolveOptions
        {
            get
            {
                return resolveOptions ?? (resolveOptions = new ResolveOptions
                {
                    UnregisteredResolutionRegistrationOption = UnregisteredResolutionRegistrationOptions.RegisterAsSingleton,
                    FallbackResolutionAction = TryAutoMock
                });
            }
        }

        protected virtual TUnit ConstructSUT()
        {
            return Get<TUnit>();
        }

        protected void EnsureSUT()
        {
            SUT = SUT;
        }

        protected T Mock<T>() where T : class
        {
            return MockRepository.GenerateMock<T>();
        }

        protected T Get<T>()
        {
            RequireMockingContainer();
            return (T)mockingContainer.Resolve(typeof(T), ResolveOptions);
            //Preferred, but only allows reference types: return MockingContainer.Resolve<T>();
        }

        protected void Set<T>(T item)
        {
            RequireMockingContainer();
            mockingContainer.Register(typeof(T), item);
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

        internal override void BeforeEachInit()
        {
            base.BeforeEachInit();

            alreadyConstructedSUT = false;
            mockingContainer = new TinyIoCContainer();

            if (typeof (TUnit).IsAbstract)
            {
                mockingContainer.Register(typeof(TUnit), (ioc, namedParameterOverloads) =>
                {
                    var constructor = ioc.GetConstructor(typeof (TUnit), namedParameterOverloads, ResolveOptions, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                    var args = ioc.ResolveConstructorParameters(constructor, namedParameterOverloads, ResolveOptions);
                    return MockRepository.GeneratePartialMock<TUnit>(args);
                }).AsSingleton();
            }
            else
            {
                mockingContainer.Register(typeof(TUnit)).AsSingleton();
            }
        }

        private void RequireMockingContainer()
        {
            if (mockingContainer == null)
            {
                throw new InvalidOperationException("This method cannot be called before the test context is initialized.");
            }
        }

        private object TryAutoMock(TinyIoCContainer.TypeRegistration registration, TinyIoCContainer container)
        {
            var type = registration.Type;
            return type.IsInterface || type.IsAbstract ? MockRepository.GenerateMock(type, new Type[0]) : null;
        }

        private TUnit GetSUTInstance()
        {
            if (!alreadyConstructedSUT)
            {
                constructedSUTInstance = ConstructSUT();

                if (constructedSUTInstance == null)
                {
                    throw new InvalidOperationException("Failed to construct SUT: ConstructSUT returned null");
                }

                if (mockingContainer.TryResolve(typeof(TUnit), new ResolveOptions{UnregisteredResolutionAction = UnregisteredResolutionActions.Fail}, out var resolvedType))
                {
                    if (!typeof(TUnit).IsValueType && !ReferenceEquals(constructedSUTInstance, resolvedType))
                    {
                        throw new InvalidOperationException("An SUT instance other than the newly constructed instance was resolved from the container");
                    }
                }
                else
                {
                    Set(constructedSUTInstance);
                }

                alreadyConstructedSUT = true;
            }

            return constructedSUTInstance;
        }
    }
}