namespace Bobbysoft.Extensions.DependencyInjection.Tests.Fakes
{
    internal class Message
    {
        public Message()
        {
        }

        public Message(string content)
        {
            Content = content;
        }

        public Message(MessageProviderBase messageProvider)
        {
            Content = messageProvider.GetMessage();
        }

        public Message(IMessageProvider messageProvider)
        {
            Content = messageProvider.GetMessage();
        }

        public string Content { get; }
    }
}
