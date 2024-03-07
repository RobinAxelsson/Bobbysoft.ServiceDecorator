# Bobbysoft.Extensions.DependencyInjection.ServiceDecoration

## ServiceDecorator

This light weight class library is used to decorate already dependency injected classes inside the Microsoft.Extensions.DependencyInjection.IServiceCollection.

---

## The quick overview

```csharp
using Bobbysoft.Extensions.DependencyInjection;

public void DecorateServices(IServiceCollection services)
{

  // ***ServiceDecorator.Decorate is the extension method added with this Library
  services.Decorate<ILogger>(
    subject => new LoggerDecorator(subject));
}

```

```csharp
using Bobbysoft.Extensions.DependencyInjection;

public void DecorateServices(IServiceCollection services)
{

  // ***ServiceDecorator.Decorate is the extension method added with this Library
  services.Decorate<ILogger>(
    (subject, provider) => new LoggerDecorator(subject, provider.GetRequiredService<Emailer>()));
}
```

---

## Step 1: make sure you know what a decorator is

- [GOF Decorator Design pattern](https://refactoring.guru/design-patterns/decorator)
- [Excellent explanation by Shiv Kumar youtube](https://www.youtube.com/watch?v=auaEZS-bAQQ)

## Step 2: decide which class to decorate

I choose the SeriLogger from SeriLog that is registered with the ILogger interface. It does not matter if the interface is an abstract class interface or an c#-Interface-Interface I could have picked any of the two for this tutorial.

## Step 3: write a decorator to the chosen abstraction

```csharp
internal class LoggerDecorator : ILogger
{
    private readonly ILogger _logger;
    public LoggerDecorator(ILogger logger)
    {
        _logger = logger;
    }

    public void Write(LogEvent logEvent)
    {
        DecorateLogEvent(logEvent);
        _logger.Write(logEvent);
    }

    private void DecorateLogEvent(LogEvent logEvent)
    {
        // Decorate your logEvent here
    }
}
```

## Step 4: Register both Service (abstraction) and implementation in the IServiceCollection

```csharp
public void ConfigureServices(IServiceCollection services)
{

  services.AddLogging(loggingBuilder =>
    loggingBuilder.AddSerilog(dispose: true));

      // Other services ...
}
```

## Step 5: Decorate the ServiceCollection from the outside of the IServiceCollection

```csharp
using Bobbysoft.Extensions.DependencyInjection;

public void DecorateServices(IServiceCollection services)
{

  // ***ServiceDecorator.Decorate is the extension method added with this Library
  services.Decorate<ILogger>(
    subject => new LoggerDecorator(subject));
}
```

---

## ***The other way - with multiple dependencies***

***Add a modified decorator***

```csharp
internal class LoggerDecorator : ILogger
{
    private readonly ILogger _logger;
    private readonly Emailer _emailer;

    // We are adding a second dependency Emailer
    public LoggerDecorator(ILogger logger, Emailer emailer)
    {
        _logger = logger;
        _emailer = emailer;
    }

    public void Write(LogEvent logEvent)
    {
        DecorateLogEvent(logEvent);
        _emailer.Send(logEvent);
        _logger.Write(logEvent);
    }

    private void DecorateLogEvent(LogEvent logEvent)
    {
        // Decorate your logEvent here
    }
}
```

***Register both implementations services***

```csharp
using Bobbysoft.Extensions.DependencyInjection;

public void ConfigureServices(IServiceCollection services)
{

  services.AddLogging(loggingBuilder =>
    loggingBuilder.AddSerilog(dispose: true));

  services.AddTransient<Emailer>();
      // Other services ...
}

public void DecorateServices(IServiceCollection services)
{

  // ***ServiceDecorator.Decorate is the extension method added with this Library
  services.Decorate<ILogger>(
    (subject, provider) => new LoggerDecorator(subject, provider.GetService<Emailer>()));
}

```

***If in doubt look at the different examples in the test project***
