
namespace SpecEasy.ExternalLib
{
    internal class MagicNumberAdder
    {
        private readonly IMagicNumberLookup magicNumberLookup;

        internal MagicNumberAdder(IMagicNumberLookup magicNumberLookup)
        {
            this.magicNumberLookup = magicNumberLookup;
        }

        internal int AddNumbers(int a, int b)
        {
            var actualA = magicNumberLookup.Lookup(a);
            var actualB = magicNumberLookup.Lookup(b);
            return actualA + actualB;
        }
    }
}
