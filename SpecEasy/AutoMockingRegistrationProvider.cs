using System;
using Rhino.Mocks;
using TinyIoC;

namespace SpecEasy
{
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