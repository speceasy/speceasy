namespace SpecEasy.Specs.TinyIoC
{
    internal class SinglePrivateCtor
    {
        public object Arg1
        {
            get;
            private set;
        }

        private SinglePrivateCtor(object arg1)
        {
            Arg1 = arg1;
        }
    }
}