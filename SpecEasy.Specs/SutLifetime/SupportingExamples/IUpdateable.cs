using System;

namespace SpecEasy.Specs.SutLifetime.SupportingExamples
{
    public interface IUpdateable
    {
        event EventHandler Updated;
    }
}