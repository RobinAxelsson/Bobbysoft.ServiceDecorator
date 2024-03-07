namespace Bobbysoft.Extensions.DependencyInjection.Tests.Fakes
{
    internal class MessageProviderDecoratorExtraDependencies : MessageProviderBase
    {
        private readonly MessageProviderBase _baseMessageProvider;
        private readonly string _extra;

        public MessageProviderDecoratorExtraDependencies(MessageProviderBase baseMessageProvider, string extra)
        {
            _baseMessageProvider = baseMessageProvider;
            _extra = extra;
        }

        public MessageProviderDecoratorExtraDependencies(MessageProviderBase baseMessageProvider, ExtraText text)
        {
            _baseMessageProvider = baseMessageProvider;
            _extra = text.Value;
        }

        public override string GetMessage()
        {
            return _extra + _baseMessageProvider.GetMessage();
        }
    }
}
