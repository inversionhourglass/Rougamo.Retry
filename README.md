# Rougamo.Retry

中文 | [English](https://github.com/inversionhourglass/Rougamo.Retry/blob/master/README_en.md)

## 快速开始

引用`Rougamo.Retry`
```
dotnet add package Rougamo.Retry
```

```csharp
// 执行 M1Async 抛出任何异常都将重试一次
[Retry]
public async Task M1Async()
{
}

// 执行 M2 抛出任何异常都将重试，最多重试三次
[Retry(3)]
public void M2()
{
}

// 执行 M3Async 抛出 IOException 或 TimeoutException 时将重试，最多重试五次
[Retry(5, typeof(IOException), typeof(TimeoutException))]
public static async ValueTask M3Async()
{
}

// 如果异常匹配逻辑复杂，可自定义类型实现 IExceptionMatcher
class ExceptionMatcher : IExceptionMatcher
{
    public bool Match(Exception e) => true;
}
[Retry(2, typeof(ExceptionMatcher))]
public static void M4()
{
}

// 如果重试的次数也是固定的，可自定义类型实现 IRetryDefinition
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

## 记录异常

有时候我们可能还希望在遇到异常时，重试的同时能够将异常信息记录到日志中，此时便可以实现`IRecordable`系列接口了。

```csharp
// 实现 IRecordableMatcher 接口将不包含重试次数定义
class RecordableMatcher : IRecordableMatcher
{
    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // 当前方法还有重试次数
        // 可通过 context.Exception 获取到当前异常
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        // 可通过 context.Exception 获取到当前异常
    }
}
[Retry(3, typeof(RecordableMatcher))]
public async ValueTask M6Async()
{
}

// 实现 IRecordableRetryDefinition 接口将包含重试次数定义
class RecordableRetryDefinition : IRecordableRetryDefinition
{
    public int Times => 3;

    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // 当前方法还有重试次数
        // 可通过 context.Exception 获取到当前异常
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        // 可通过 context.Exception 获取到当前异常
    }
}
[Retry(typeof(RecordableRetryDefinition))]
public async Task M7Async()
{
}
```

### 异步异常处理

5.0 版本为 netstandard2.1 下的`IRecordable`增加了异步异常处理方法`TemporaryFailedAsync`和`UltimatelyFailedAsync`，方便进行异步操作。只为 netstandard2.1 增加异步方法，是因为 netstandard2.1 支持默认接口方法，所以`ISyncRecordable`和`IAsyncRecordable`，两个接口分别实现了默认的异步方法和默认的同步方法，方便使用。同时，由于`IRecordableMatcher`和`IRecordableRetryDefinition`也实现了`IRecordable`接口，所以也为他们增加了实现了默认方法的同步/异步接口。

### 依赖注入

记录异常的方式有很多种，比较常用的应该就是写入日志了，而很多日志框架都是需要依赖注入支持的，而`Rougamo.Retry`本身是没有依赖注入功能的，上面定义的类型都将使用无参构造方法创建其对象。但`Rougamo.Retry`提供了修改对象创建方式的方法`Resolver.Set(ResolverFactory)`。推荐结合 [`DependencyInjection.StaticAccessor`](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor) 完成依赖注入设置。

```csharp
// 修改对象创建方式，从 PinnedScope.ScopedServices 中获取对象实例
Resolver.Set(t => PinnedScope.ScopedServices!.GetRequiredService(t));

// 后续初始化请根据你的项目类型，参照 DependencyInjection.StaticAccessor 系列文档完成
```

#### ~~Rougamo.Retry.AspNetCore~~

<font color=red>**注意，Rougamo.Retry.AspNetCore 已不再推荐，相关 NuGet 包将标记为过期，推荐参照 [依赖注入](#依赖注入) 开始部分介绍的 [`DependencyInjection.StaticAccessor`](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor) 完成依赖注入。**</font>

```csharp
// 1. 定义实现 IRecordableMatcher 或 IRecordableRetryDefinition 的类型，并注入和使用 ILogger
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
        // 当前方法还有重试次数
        _logger.LogDebug(context.Exception, string.Empty);
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        _logger.LogError(context.Exception, string.Empty);
    }
}

// 2. 在 Startup 中进行初始化
class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 2.1. 将获取对象的工厂改为 IServiceProvider
        services.AddAspNetRetryFactory();
        // 2.2. 注册 RecordableRetryDefinition
        services.AddTransient<RecordableRetryDefinition>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 2.3. 注册相关的 Middleware，尽量放到前面，否则如果前面的 Middleware 有用到 Rougamo.Retry 可能会出现异常
        app.UseRetryFactory();
    }
}

// 3. 使用，使用还是和之前一样，但是这里的 RecordableRetryDefinition 就可以使用依赖注入了
[Retry(typeof(RecordableRetryDefinition))]
public static async Task M8Async()
{
}
```

#### ~~Rougamo.Retry.GeneralHost~~

<font color=red>**注意，Rougamo.Retry.GeneralHost 已不再推荐，相关 NuGet 包将标记为过期，推荐参照 [依赖注入](#依赖注入) 开始部分介绍的 [`DependencyInjection.StaticAccessor`](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor) 完成依赖注入。**</font>

除了 AspNetCore，我们可能还会创建一些通用程序，此时就可以直接引用`Rougamo.Retry.GeneralHost`了
```csharp
// 1. 定义实现 IRecordableMatcher 或 IRecordableRetryDefinition 的类型，并注入和使用 ILogger
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
        // 当前方法还有重试次数
        _logger.LogDebug(context.Exception, string.Empty);
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        _logger.LogError(context.Exception, string.Empty);
    }
}

// 2. 在 ConfigureServices 中进行初始化
public void ConfigureServices(IServiceCollection services)
{
    // 2.1. 将获取对象的工厂改为 IServiceProvider
    services.AddRetryFactory();
    // 2.2. 注册 RecordableMatcher
    services.AddTransient<RecordableMatcher>();
}

// 3. 使用
[Retry(2, typeof(RecordableMatcher))]
public static void M9()
{
}
```

**需要注意的是，AspNetCore 的实现比 GeneralHost 稍微复杂一点，原因是 AspNetCore 中一般有一些类型会注册为 Scoped 生命周期，所以 AspNetCore 中会额外注册一个 Middleware 处理 IServiceProvider 的获取逻辑，如果你的通用程序中也存在 Scoped 生命周期，并且实现 Rougamo.Retry 的相关接口时注入了 Scoped 类型，那么你需要参考`Rougamo.Retry.AspNetCore`也进行一些额外的处理**

### 统一记录异常

在了解 [记录异常](#记录异常) 之后可能会想到一个问题，如果记录异常的逻辑简单通用，那么每次实现`IRecordableMatcher`或`IRecordableRetryDefinition`接口时都要带上这段逻辑处理会有些麻烦，虽然说可以抽象父类，但还是会稍显麻烦而且有遗漏的可能。

考虑到这个问题`Rougamo.Retry`也提供了统一记录异常的方式，那就是`RecordRetryAttribute`和`IRecordable`的组合。

```csharp
// 1. 实现 IRecordable 接口
class Recordable : IRecordable
{
    private readonly ILogger _logger;

    public Recordable(ILogger<Recordable> logger)
    {
        _logger = logger;
    }

    public void TemporaryFailed(ExceptionContext context)
    {
        // 当前方法还有重试次数
        _logger.LogDebug(context.Exception, string.Empty);
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        _logger.LogError(context.Exception, string.Empty);
    }
}

// 2. 注册 Recordable，注意这里只展示了额外的步骤，如果你使用了 Rougamo.Retry.AspNetCore 或 Rougamo.Retry.GeneralHost，那么你同样需要完成这些组件各自的初始化操作
public void ConfigureServices(IServiceCollection services)
{
    services.AddRecordable<Recordable>();
}

// 3. 使用，以下操作都会自动执行 Recordable 的异常记录动作
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

## 注意

- 在使用`RetryAttribute`和`RecordRetryAttribute`时，当前项目必须直接引用`Rougamo.Retry`，不可间接引用，否则代码无法织入。
