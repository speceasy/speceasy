using System;

namespace SpecEasy.Specs.Disposal.SupportingExamples
{
    public interface IUpdateable
    {
        event EventHandler Updated;
    }
}