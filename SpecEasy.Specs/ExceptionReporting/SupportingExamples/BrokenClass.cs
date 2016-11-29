using System;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    internal class BrokenClass
    {
        public bool InvertThatThrowsArgumentOutOfRangeException(bool valueToInvert)
        {
            throw new ArgumentOutOfRangeException("valueToInvert", "exception while trying to invert value");
        }

        public bool InvertThatDoesNotThrow(bool valueToInvert)
        {
            return !valueToInvert;
        }
    }
}