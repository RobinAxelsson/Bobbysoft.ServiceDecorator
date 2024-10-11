using Bobbysoft.Extensions.DependencyInjection;
using Bobbysoft.ServiceDecorator.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Bobbysoft.ServiceDecorator.Tests
{
    //For more theory see Decorator Design Pattern
    public class ServiceDecoratorTests
    {
        [Fact]
        public void Decorate_generic_descriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddTransient<Message>();

            // Creates a "generic" descriptor
            services.AddTransient<MessageProviderBase, HelloMessageProvider>();

            // Act
            services.Decorate<MessageProviderBase>(subject => new MessageProviderDecorator(subject));
            var message = services.BuildServiceProvider().GetRequiredService<Message>();

            // Assert
            message.Content.Should().Be("Decorated: Hello");
        }

        [Fact]
        public void Decorate_single_generic_descriptor_should_throw()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<HelloMessageProvider>();

            // Act & Assert
            FluentActions.Invoking(
                    () => services.Decorate<HelloMessageProvider>(subject => new HelloMessageProviderDecorator(subject)))
                .Should().Throw<ServiceDecorationException>();
        }

        [Fact]
        public void Decorate_abstract_subject_more_then_one_descriptor_per_service_type_should_throw()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<MessageProviderBase, HelloMessageProvider>();
            services.AddScoped<MessageProviderBase, MessageProviderDecorator>();

            // Act & Assert
            FluentActions.Invoking(
                    () => services.Decorate<MessageProviderBase>(subject => new MessageProviderDecorator(subject)))
                .Should().Throw<ServiceDecorationException>();
        }

        [Fact]
        public void Decorate_non_abstract_subject_with_object_descriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Letter>();
            services.AddSingleton(new HelloMessageProvider());

            // Act
            services.Decorate<HelloMessageProvider>(subject =>
                new HelloMessageProviderDecorator(subject));

            var provider = services.BuildServiceProvider();

            // Assert
            var messageService = provider.GetRequiredService<Letter>();
            messageService.Content.Should().Be("HelloHello");
        }

        [Fact]
        public void Decorate_non_abstract_subject_with_factory_descriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Letter>();
            services.AddSingleton(_ => new HelloMessageProvider());

            // Act
            services.Decorate<HelloMessageProvider>(subject =>
                new HelloMessageProviderDecorator(subject));

            var provider = services.BuildServiceProvider();

            // Assert
            var messageService = provider.GetRequiredService<Letter>();
            messageService.Content.Should().Be("HelloHello");
        }

        [Fact]
        public void Decorate_factory_descriptor()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Message>();
            services.AddSingleton<MessageProviderBase>(
                _ => new HelloMessageProvider());

            // Act
            services.Decorate<MessageProviderBase>(subject =>
                new MessageProviderDecorator(subject));

            var provider = services.BuildServiceProvider();
            var messageService = provider.GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Decorated: Hello");
        }

        [Fact]
        public void Decorate_generic_descriptor_twice_with_new()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Message>();
            services.AddSingleton<ExtraText>();
            services.AddSingleton<MessageProviderBase, HelloMessageProvider>();

            // Act
            services.Decorate<MessageProviderBase>(subject =>
                new MessageProviderDecorator(
                    new MessageProviderDecorator(subject)));

            var provider = services.BuildServiceProvider();
            var messageService = provider.GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Decorated: Decorated: Hello");
        }

        [Fact]
        public void Decorate_generic_descriptor_with_provider()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddScoped<Message>();
            services.AddScoped<ExtraText>();
            services.AddScoped<MessageProviderBase, HelloMessageProvider>();

            // Act
            services.Decorate<MessageProviderBase>((subject, provider) =>
                new MessageProviderDecoratorExtraDependencies(subject, provider.GetRequiredService<ExtraText>()));

            var messageService = services.BuildServiceProvider().GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Extra injected: Hello");
        }

        [Fact]
        public void Decorate_object_descriptor_with_provider()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Message>();
            services.AddSingleton<MessageProviderBase>(new HelloMessageProvider());

            // Act
            services.Decorate<MessageProviderBase>((subject, provider) =>
                new MessageProviderDecorator(subject));

            var messageService = services.BuildServiceProvider().GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Decorated: Hello");
        }

        [Fact]
        public void Decorate_factory_descriptor_with_provider()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Message>();
            services.AddSingleton<MessageProviderBase>(provider => new HelloMessageProvider());

            // Act
            services.Decorate<MessageProviderBase>((subject, provider) =>
                new MessageProviderDecorator(subject));

            var messageService = services.BuildServiceProvider().GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Decorated: Hello");
        }

        [Fact]
        public void Decorate_provider_factory_descriptor_repeatedly()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Message>();
            services.AddSingleton<MessageProviderBase>(provider => new HelloMessageProvider());

            // Act
            services.Decorate<MessageProviderBase>((subject, provider) =>
                new MessageProviderDecorator(subject));

            services.Decorate<MessageProviderBase>((subject, provider) =>
                new MessageProviderDecorator(subject));

            services.Decorate<MessageProviderBase>((subject, provider) =>
                new MessageProviderDecorator(subject));

            var messageService = services.BuildServiceProvider().GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Decorated: Decorated: Decorated: Hello");
        }

        [Fact]
        public void Decorate_generic_descriptor_repeatedly()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddSingleton<Message>();
            services.AddSingleton<MessageProviderBase, HelloMessageProvider>();

            // Act
            services.Decorate<MessageProviderBase>(subject =>
                new MessageProviderDecorator(subject));

            services.Decorate<MessageProviderBase>(subject =>
                new MessageProviderDecorator(subject));

            services.Decorate<MessageProviderBase>(subject =>
                new MessageProviderDecorator(subject));

            var messageService = services.BuildServiceProvider().GetRequiredService<Message>();

            // Assert
            messageService.Content.Should().Be("Decorated: Decorated: Decorated: Hello");
        }
    }
}
