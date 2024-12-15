# Rougamo.Retry

[中文](https://github.com/inversionhourglass/Rougamo.Retry/blob/master/README.md) | English

## Use Rougamo.Retry

```
dotnet add package Rougamo.Retry
```

## Quick Start

```csharp
// Any exception thrown by M1Async will be retried once
[Retry]
public async Task M1Async()
{
}

// Any exception thrown by M2 will be retried, up to three times
[Retry(3)]
public void M2()
{
}

// When M3Async throws IOException or TimeoutException, it will retry up to five times
[Retry(5, typeof(IOException), typeof(TimeoutException))]
public static async ValueTask M3Async()
{
}

// If the exception matching logic is complex, you can customize the type to implement IExceptionMatcher
class ExceptionMatcher : IExceptionMatcher
{
    public bool Match(Exception e) => true;
}
[Retry(2, typeof(ExceptionMatcher))]
public static void M4()
{
}

// If the number of retries is also fixed, you can customize the type to implement IRetryDefinition
class RetryDefinition : IRetryDefinition
{
    public int Times => 3;

    public bool Match(Exception e) => true;
}
[Retry(typeof(RetryDefinition))]
public void M5()
{
}
```

## Record Exception

Sometimes we also want to record the exception information when the exception throws. At this time, we can implement `IRecordable`.
```csharp
// Implemente the IRecordableMatcher will not include the definition of the number of retries
class RecordableMatcher : IRecordableMatcher
{
    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // The current method still has the number of retries
        // The current exception can be obtained through context.Exception
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // The number of retries for the current method has been used up, and finally failed
        // The current exception can be obtained through context.Exception
    }
}
[Retry(3, typeof(RecordableMatcher))]
public async ValueTask M6Async()
{
}

// Implemente the IRecordableRetryDefinition will include the definition of the number of retries
class RecordableRetryDefinition : IRecordableRetryDefinition
{
    public int Times => 3;

    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // The current method still has the number of retries
        // The current exception can be obtained through context.Exception
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // The number of retries for the current method has been used up, and finally failed
        // The current exception can be obtained through context.Exception
    }
}
[Retry(typeof(RecordableRetryDefinition))]
public async Task M7Async()
{
}
```

### Asynchronous Exception Handling

Version 5.0 added asynchronous exception handling methods `TemporaryFailedAsync` and `UltimatelyFailedAsync` to the `IRecordable` interface for netstandard2.1, facilitating asynchronous operations. The asynchronous methods were only added for netstandard2.1 because it supports default interface methods. Therefore, the `ISyncRecordable` and `IAsyncRecordable` interfaces have implemented default asynchronous and synchronous methods respectively, for easier use. Additionally, since `IRecordableMatcher` and `IRecordableRetryDefinition` also implement the `IRecordable` interface, they have been provided with implementations of the default synchronous/asynchronous methods as well.

### Dependency Injection

The methods for recording exceptions are varied, with writing to logs being one of the most commonly used. Many logging frameworks rely on dependency injection support, whereas `Rougamo.Retry` does not come with built-in dependency injection capabilities. The types defined above will use parameterless constructors to create their objects. However, `Rougamo.Retry` offers a method to alter the object creation process via `Resolver.Set(ResolverFactory)`. It is recommended to combine this with [`DependencyInjection.StaticAccessor`](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor) to set up dependency injection.

```csharp
// Modify the object creation method to obtain object instances from PinnedScope.ScopedServices
Resolver.Set(t => PinnedScope.ScopedServices!.GetRequiredService(t));

// Subsequent initialization should be completed according to the series of documentation for DependencyInjection.StaticAccessor, based on your project type.
```

#### ~~Rougamo.Retry.AspNetCore~~

<font color=red>**Note that `Rougamo.Retry.AspNetCore` is no longer recommended, and the related NuGet package will be marked as obsolete. It is suggested to follow the introduction provided in the [Dependency Injection](#dependency-injection) section and use [`DependencyInjection.StaticAccessor`](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor) to set up dependency injection.**</font>

```csharp
// 1. Define a type that implements IRecordableMatcher or IRecordableRetryDefinition, then inject and use ILogger
class RecordableRetryDefinition : IRecordableRetryDefinition
{
    private readonly ILogger _logger;

    public RecordableRetryDefinition(ILogger<RecordableRetryDefinition> logger)
    {
        _logger = logger;
    }

    public int Times => 3;

    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // The current method still has the number of retries
        _logger.LogDebug(context.Exception, string.Empty);
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // The number of retries for the current method has been used up, and finally failed
        _logger.LogError(context.Exception, string.Empty);
    }
}

// 2. Initialize in Startup
class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 2.1. Replace the retry factory to IServiceProvider
        services.AddAspNetRetryFactory();
        // 2.2. Register RecordableRetryDefinition
        services.AddTransient<RecordableRetryDefinition>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 2.3. Register the middleware of Rougamo.Retry.AspNetCore. Try to put it in the front, otherwise if the previous middlewares uses Rougamo.Retry, then an exception may occur.
        app.UseRetryFactory();
    }
}

// 3. Apply RetryAttribute to methods
[Retry(typeof(RecordableRetryDefinition))]
public static async Task M8Async()
{
}
```

#### Rougamo.Retry.GeneralHost

<font color=red>**Note that `Rougamo.Retry.GeneralHost` is no longer recommended, and the related NuGet package will be marked as obsolete. It is suggested to follow the introduction provided in the [Dependency Injection](#dependency-injection) section and use [`DependencyInjection.StaticAccessor`](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor) to set up dependency injection.**</font>

In addition to AspNetCore, we may also create some general programs. At this time, we need to reference `Rougamo.Retry.GeneralHost`.
```csharp
// 1. Define a type that implements IRecordableMatcher or IRecordableRetryDefinition, then inject and use ILogger
class RecordableMatcher : IRecordableMatcher
{
    private readonly ILogger _logger;

    public RecordableMatcher(ILogger<RecordableMatcher> logger)
    {
        _logger = logger;
    }

    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // The current method still has the number of retries
        _logger.LogDebug(context.Exception, string.Empty);
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // The number of retries for the current method has been used up, and finally failed
        _logger.LogError(context.Exception, string.Empty);
    }
}

// 2. Initialize in ConfigureServices
public void ConfigureServices(IServiceCollection services)
{
    // 2.1. Replace the retry factory to IServiceProvider
    services.AddRetryFactory();
    // 2.2. Register RecordableMatcher
    services.AddTransient<RecordableMatcher>();
}

// 3. Apply RetryAttribute to methods
[Retry(2, typeof(RecordableMatcher))]
public static void M9()
{
}
```

### Unified record Exception
If the logic for recording exceptions is generic, then we can simplify this operation with `RecordRetryAttribute` and `IRecordable`.
```csharp
// 1. Define a type that implements IRecordable
class Recordable : IRecordable
{
    private readonly ILogger _logger;

    public Recordable(ILogger<Recordable> logger)
    {
        _logger = logger;
    }

    public void TemporaryFailed(ExceptionContext context)
    {
        // The current method still has the number of retries
        _logger.LogDebug(context.Exception, string.Empty);
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // The number of retries for the current method has been used up, and finally failed
        _logger.LogError(context.Exception, string.Empty);
    }
}

// 2. Register Recordable. Note that only additional steps are shown here, if you use Rougamo.Retry.AspNetCore or Rougamo.Retry.GeneralHost, then you also need to complete the initialization of these components
public void ConfigureServices(IServiceCollection services)
{
    services.AddRecordable<Recordable>();
}

// 3. Apply RecordRetryAttribute to methods
[RecordRetry]
public async Task M10Async() { }

[RecordRetry(3)]
public void M11() { }

[RecordRetry(5, typeof(IOException), typeof(TimeoutException))]
public static async ValueTask M12Async() { }

class ExceptionMatcher : IExceptionMatcher
{
    public bool Match(Exception e) => true;
}
[RecordRetry(2, typeof(ExceptionMatcher))]
public static void M13() { }

class RetryDefinition : IRetryDefinition
{
    public int Times => 3;

    public bool Match(Exception e) => true;
}
[RecordRetry(typeof(RetryDefinition))]
public void M14()
{
}
```

## Attention

- When using `RetryAttribute` and `RecordRetryAttribute`, the current project must directly reference `Rougamo.Retry`, not indirect reference, otherwise the code cannot be woven.
