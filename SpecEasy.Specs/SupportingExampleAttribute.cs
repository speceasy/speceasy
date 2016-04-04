using NUnit.Framework;

namespace SpecEasy.Specs
{
    public class SupportingExampleAttribute : ExplicitAttribute
    {
        public SupportingExampleAttribute()
            : base("These tests are part of a supporting example and are not intended to be run directly")
        {
        }
    }
}