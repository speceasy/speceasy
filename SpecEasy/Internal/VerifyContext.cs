using System;

namespace SpecEasy.Internal
{
    internal class VerifyContext : IVerifyContext
    {
        private readonly Func<string, Action, IVerifyContext> addSpec;

        public VerifyContext(Func<string, Action, IVerifyContext> addSpec)
        {
            this.addSpec = addSpec;
        }

        public IVerifyContext Then(string description, Action specification)
        {
            return addSpec(description, specification);
        }
    }
}