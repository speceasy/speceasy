using System;

namespace SpecEasy.Specs.SutLifetime.SupportingExamples
{
    public sealed class DisposableCounter : IDisposable
    {
        public int DisposeCount
        {
            get;
            private set;
        }

        public void Dispose()
        {
            DisposeCount++;
        }
    }
}
