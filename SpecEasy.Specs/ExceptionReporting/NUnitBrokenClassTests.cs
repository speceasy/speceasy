using NUnit.Framework;

namespace Examples
{
    [TestFixture]
    internal class NUnitBrokenClassTests
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