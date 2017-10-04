using System;

namespace SpecEasy.Specs.Disposal.SupportingExamples
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
