namespace SpecEasy.Specs.TinyIoC
{
    internal class TwoPublicCtors
    {
        public object Arg1
        {
            get;
            private set;
        }

        public object Arg2
        {
            get;
            private set;
        }

        public TwoPublicCtors(object arg1)
        {
            Arg1 = arg1;
        }

        public TwoPublicCtors(object arg1, object arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }
}