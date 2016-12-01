namespace SpecEasy
{
    public interface IVerifyContext
    {
        IVerifyContext Then(string description, Action specification);
    }
}