namespace Bobbysoft.ServiceDecorator.Tests.Fakes
{
    internal class Letter
    {
        public Letter(HelloMessageProvider messageProvider)
        {
            Content = messageProvider.GetMessage();
        }

        public string Content { get; }
    }
}
