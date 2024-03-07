namespace Bobbysoft.Extensions.DependencyInjection.Tests.Fakes
{
    internal class HelloMessageProvider : MessageProviderBase
    {
        public override string GetMessage()
        {
            return "Hello";
        }
    }
}
