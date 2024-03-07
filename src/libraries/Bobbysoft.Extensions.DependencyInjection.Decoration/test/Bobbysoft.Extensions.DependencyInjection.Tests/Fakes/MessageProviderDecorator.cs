namespace Bobbysoft.Extensions.DependencyInjection.Tests.Fakes
{
    internal class MessageProviderDecorator : MessageProviderBase
    {
        private readonly MessageProviderBase _provider;

        public MessageProviderDecorator(MessageProviderBase provider)
        {
            _provider = provider;
        }

        public override string GetMessage()
        {
            return "Decorated: " + _provider.GetMessage();
        }
    }
}
