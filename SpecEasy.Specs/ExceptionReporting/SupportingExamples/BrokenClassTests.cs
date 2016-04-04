using NUnit.Framework;

namespace SpecEasy.Specs.ExceptionReporting.SupportingExamples
{
    [TestFixture, SupportingExample]
    internal class BrokenClassTests
    {
        [Test]
        public void GivenFalseItReturnsTrue()
        {
            var result = new BrokenClass().Invert(false);
            Assert.IsTrue(result);
        }

        [Test]
        public void GivenTrueItReturnsFalse()
        {
            var result = new BrokenClass().Invert(true);
            Assert.IsFalse(result);
        }
    }
}