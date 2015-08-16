namespace SpecEasy.Specs.TinyIoC
{
    internal class PublicAndPrivateCtors
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

        private PublicAndPrivateCtors(object arg1)
        {
            Arg1 = arg1;
        }

        public PublicAndPrivateCtors(object arg1, object arg2)
        {
            Arg1 = arg1;
            Arg2 = arg2;
        }
    }
}