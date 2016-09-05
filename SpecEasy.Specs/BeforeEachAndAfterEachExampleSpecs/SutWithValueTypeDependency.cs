namespace SpecEasy.Specs.BeforeEachAndAfterEachExampleSpecs
{
    internal class SutWithValueTypeDependency
    {
        public int Value { get; private set; }

        public SutWithValueTypeDependency(int value)
        {
            Value = value;
        }
    }
}