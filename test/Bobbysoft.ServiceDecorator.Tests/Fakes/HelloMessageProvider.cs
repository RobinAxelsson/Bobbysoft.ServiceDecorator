namespace Bobbysoft.ServiceDecorator.Tests.Fakes
{
    internal class HelloMessageProvider : MessageProviderBase
    {
        public override string GetMessage()
        {
            return "Hello";
        }
    }
}
