using System;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    internal class BrokenClass
    {
        public bool Invert(bool value)
        {
            throw new Exception("exception while trying to invert value");
        }
    }
}