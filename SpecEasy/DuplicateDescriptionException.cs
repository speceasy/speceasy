using System;

namespace SpecEasy
{
    public class DuplicateDescriptionException : InvalidOperationException
    {
        public override string StackTrace
        {
            get
            {
                return stackTrace;
            }
        }

        private readonly string stackTrace;

        internal DuplicateDescriptionException(string typeOfDuplicate, string description, string stackTrace)
            : base(string.Format("Failed to generate test cases; {0} description '{1}' has already been used", typeOfDuplicate, description))
        {
            this.stackTrace = stackTrace;
        }
    }
}