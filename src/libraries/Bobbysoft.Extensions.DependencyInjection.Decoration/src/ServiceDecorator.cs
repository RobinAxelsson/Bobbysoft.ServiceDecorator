using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Bobbysoft.Extensions.DependencyInjection
{
public static class ServiceDecorator
{
    public static void Decorate<T>(this IServiceCollection services, Func<T, IServiceProvider, T> implementationFactory)
    {
        var subjectDescriptor = RetrieveSubjectDescriptor<T>(services);
        var decoratedDescriptor = CreateDecoratedServiceDescriptor(services, implementationFactory, subjectDescriptor);
        UpdateServiceCollection(services, decoratedDescriptor, subjectDescriptor);
    }

    public static void Decorate<T>(this IServiceCollection services, Func<T, T> implementationFactory)
    {
        var subjectDescriptor = RetrieveSubjectDescriptor<T>(services);
        var decoratedDescriptor =
            CreateDecoratedServiceDescriptor(services, FactoryAdapter(implementationFactory), subjectDescriptor);
        UpdateServiceCollection(services, decoratedDescriptor, subjectDescriptor);
    }

    private static ServiceDescriptor CreateDecoratedServiceDescriptor<T>(
        IServiceCollection services,
        Func<T, IServiceProvider, T> implementationFactory,
        ServiceDescriptor subjectDescriptor)
    {
        ServiceDescriptor newDescriptor = null!;

        if (ObjectDescriptor(subjectDescriptor))
        {
            newDescriptor = ObjectDescriptorCreate(implementationFactory,
                (T) subjectDescriptor.ImplementationInstance!, subjectDescriptor.Lifetime);
        }

        else if (FactoryDescriptor(subjectDescriptor))
        {
            newDescriptor = FactoryDecoratorDescriptorCreate(
                implementationFactory, subjectDescriptor.ImplementationFactory!, subjectDescriptor.Lifetime);
        }

        else if (GenericsDescriptor(subjectDescriptor))
        {
            newDescriptor = GenericsDecoratorDescriptorCreate(implementationFactory,
                subjectDescriptor.ImplementationType!, subjectDescriptor.Lifetime);
            RepointSubjectDescriptor(services, subjectDescriptor.ImplementationType!, subjectDescriptor.Lifetime);
        }

        return newDescriptor;
    }

    private static ServiceDescriptor RetrieveSubjectDescriptor<T>(IServiceCollection services)
    {
        ServiceDescriptor descriptor;

        try
        {
            descriptor = services.Single(x => x.ServiceType == typeof(T));
        }
        catch (Exception e)
        {
            var count = services.Count(x => x.ServiceType == typeof(T));
            throw new ServiceDecorationException("To be able to decorate exactly one descriptor is allowed with" +
                                                 $" the target service. Target: {typeof(T).Name} count: {count}", e);
        }

        if (ImplementationSameAsServiceType(descriptor))
            throw new ServiceDecorationException(
                "You must not decorate a descriptor with same service type as " +
                $"implementation, you can only decorate an abstracted type");

        return descriptor!;
    }

    private static Func<T, IServiceProvider, T> FactoryAdapter<T>(Func<T, T> implementationFactory)
    {
        return (subject, provider) => implementationFactory(subject);
    }

    private static void UpdateServiceCollection(IServiceCollection services, ServiceDescriptor decoratedDescriptor,
        ServiceDescriptor subjectDescriptor)
    {
        services.Add(decoratedDescriptor);
        services.Remove(subjectDescriptor);
    }

    private static ServiceDescriptor FactoryDecoratorDescriptorCreate<T>(
        Func<T, IServiceProvider, T> decoration, Func<IServiceProvider, object> implementationFactory,
        ServiceLifetime lifetime)
    {
        object Factory(IServiceProvider provider)
        {
            var subject = (T) implementationFactory(provider);
            return decoration(subject, provider)!;
        }

        return new ServiceDescriptor(typeof(T), Factory, lifetime);
    }

    private static ServiceDescriptor GenericsDecoratorDescriptorCreate<T>(
        Func<T, IServiceProvider, T> decoration, Type implementationType, ServiceLifetime lifetime)
    {
        object Factory(IServiceProvider provider)
        {
            var subject = GetRequiredImplementation<T>(provider, implementationType);
            return decoration(subject, provider)!;
        }

        return new ServiceDescriptor(typeof(T), Factory, lifetime);
    }

    private static ServiceDescriptor ObjectDescriptorCreate<T>(
        Func<T, IServiceProvider, T> decoration,
        T instance,
        ServiceLifetime lifetime)
    {
        object Factory(IServiceProvider provider)
        {
            return decoration(instance, provider)!;
        }

        return new ServiceDescriptor(typeof(T), Factory, lifetime);
    }

    private static T GetRequiredImplementation<T>(IServiceProvider provider, Type implementationType)
    {
        T subject;
        try
        {
            subject = (T) provider.GetRequiredService(implementationType);
        }
        catch (Exception e)
        {
            throw new ServiceDecorationException(
                $"Failed to get required service subject to decorate. ServiceType: {typeof(T).Name}", e);
        }

        return subject;
    }

    private static bool ImplementationSameAsServiceType(ServiceDescriptor descriptor)
    {
        return descriptor.ImplementationType != null && descriptor.ServiceType == descriptor.ImplementationType;
    }

    private static void RepointSubjectDescriptor(
        IServiceCollection services, Type descriptorImplementationType, ServiceLifetime descriptorLifetime)
    {
        services.Add(new ServiceDescriptor(descriptorImplementationType, descriptorImplementationType,
            descriptorLifetime));
    }

    private static bool ObjectDescriptor(ServiceDescriptor descriptor)
    {
        return descriptor.ImplementationInstance != null;
    }

    private static bool FactoryDescriptor(ServiceDescriptor descriptor)
    {
        return descriptor.ImplementationFactory != null;
    }

    private static bool GenericsDescriptor(ServiceDescriptor descriptor)
    {
        return descriptor.ImplementationType != null;
    }
}
}
