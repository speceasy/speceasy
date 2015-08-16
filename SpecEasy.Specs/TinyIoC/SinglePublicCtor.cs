namespace SpecEasy.Specs.TinyIoC
{
    internal class SinglePublicCtor
    {
        public object Arg1
        {
            get;
            private set;
        }

        public SinglePublicCtor(object arg1)
        {
            Arg1 = arg1;
        }
    }
}