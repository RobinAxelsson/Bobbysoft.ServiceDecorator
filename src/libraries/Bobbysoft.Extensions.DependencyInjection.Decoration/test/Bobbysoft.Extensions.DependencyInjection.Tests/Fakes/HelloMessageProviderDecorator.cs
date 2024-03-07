namespace Bobbysoft.Extensions.DependencyInjection.Tests.Fakes
{
    internal class HelloMessageProviderDecorator : HelloMessageProvider
    {
        private readonly HelloMessageProvider _helloMessageProvider;

        public HelloMessageProviderDecorator(HelloMessageProvider helloMessageProvider)
        {
            _helloMessageProvider = helloMessageProvider;
        }

        public override string GetMessage()
        {
            return "Hello" + _helloMessageProvider.GetMessage();
        }
    }
}
