# Rougamo.Retry
中文 | [English](README_en.md)

## 引用Rougamo.Retry
```
dotnet add package Rougamo.Retry
```

## 快速开始
```csharp
// 执行M1Async抛出任何异常都将重试一次
[Retry]
public async Task M1Async()
{
}

// 执行M2抛出任何异常都将重试，最多重试三次
[Retry(3)]
public void M2()
{
}

// 执行M3Async抛出IOException或TimeoutException时将重试，最多重试五次
[Retry(5, typeof(IOException), typeof(TimeoutException))]
public static async ValueTask M3Async()
{
}

// 如果异常匹配逻辑复杂，可自定义类型实现IExceptionMatcher
class ExceptionMatcher : IExceptionMatcher
{
    public bool Match(Exception e) => true;
}
[Retry(2, typeof(ExceptionMatcher))]
public static void M4()
{
}

// 如果重试的次数也是固定的，可自定义类型实现IRetryDefinition
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
有时候我们可能还希望在遇到异常时，重试的同时能够将异常信息记录到日志中，此时便可以实现`IRecordable`系列接口了
```csharp
// 实现IRecordableMatcher接口将不包含重试次数定义
class RecordableMatcher : IRecordableMatcher
{
    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // 当前方法还有重试次数
        // 可通过context.Exception获取到当前异常
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        // 可通过context.Exception获取到当前异常
    }
}
[Retry(3, typeof(RecordableMatcher))]
public async ValueTask M6Async()
{
}

// 实现IRecordableRetryDefinition接口将包含重试次数定义
class RecordableRetryDefinition : IRecordableRetryDefinition
{
    public int Times => 3;

    public bool Match(Exception e) => true;

    public void TemporaryFailed(ExceptionContext context)
    {
        // 当前方法还有重试次数
        // 可通过context.Exception获取到当前异常
    }

    public void UltimatelyFailed(ExceptionContext context)
    {
        // 当前方法重试次数已用完，最终还是执行失败了
        // 可通过context.Exception获取到当前异常
    }
}
[Retry(typeof(RecordableRetryDefinition))]
public async Task M7Async()
{
}
```

### 依赖注入
记录异常的方式有很多种，比较常用的应该就是写入日志了，而很多日志框架都是需要依赖注入支持的，而`Rougamo.Retry`本身是没有依赖注入功能的，上面定义的类型都将使用无参构造方法创建其对象。
考虑到依赖注入的普遍性，这里增加了两个扩展项目`Rougamo.Retry.AspNetCore`和`Rougamo.Retry.GeneralHost`。

#### Rougamo.Retry.AspNetCore
```csharp
// 1. 定义实现IRecordableMatcher或IRecordableRetryDefinition的类型，并注入和使用ILogger
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

// 2. 在Startup中进行初始化
class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // 2.1. 将获取对象的工厂改为IServiceProvider
        services.AddAspNetRetryFactory();
        // 2.2. 注册RecordableRetryDefinition
        services.AddTransient<RecordableRetryDefinition>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // 2.3. 注册相关的Middleware，尽量放到前面，否则如果前面的Middleware有用到Rougamo.Retry可能会出现异常
        app.UseRetryFactory();
    }
}

// 3. 使用，使用还是和之前一样，但是这里的RecordableRetryDefinition就可以使用依赖注入了
[Retry(typeof(RecordableRetryDefinition))]
public static async Task M8Async()
{
}
```

#### Rougamo.Retry.GeneralHost
除了AspNetCore，我们可能还会创建一些通用程序，此时就可以直接引用`Rougamo.Retry.GeneralHost`了
```csharp
// 1. 定义实现IRecordableMatcher或IRecordableRetryDefinition的类型，并注入和使用ILogger
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

// 2. 在ConfigureServices中进行初始化
public void ConfigureServices(IServiceCollection services)
{
    // 2.1. 将获取对象的工厂改为IServiceProvider
    services.AddRetryFactory();
    // 2.2. 注册RecordableMatcher
    services.AddTransient<RecordableMatcher>();
}

// 3. 使用
[Retry(2, typeof(RecordableMatcher))]
public static void M9()
{
}
```
**需要注意的是，AspNetCore的实现比GeneralHost稍微复杂一点，原因是AspNetCore中一般有一些类型会注册为Scoped生命周期，所以AspNetCore中会额外注册一个Middleware处理IServiceProvider的获取逻辑，如果你的通用程序中也存在Scoped生命周期，并且实现Rougamo.Retry的相关接口时注入了Scoped类型，那么你需要参考`Rougamo.Retry.AspNetCore`也进行一些额外的处理**

### 统一记录异常
在了解 [记录异常](#记录异常) 之后可能会想到一个问题，如果记录异常的逻辑简单通用，那么每次实现`IRecordableMatcher`或`IRecordableRetryDefinition`接口时都要带上这段逻辑处理会有些麻烦，虽然说可以抽象父类，但还是会稍显麻烦而且有遗漏的可能。
考虑到这个问题`Rougamo.Retry`也提供了统一记录异常的方式，那就是`RecordRetryAttribute`和`IRecordable`的组合。
```csharp
// 1. 实现IRecordable接口
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

// 2. 注册Recordable，注意这里只展示了额外的步骤，如果你使用了Rougamo.Retry.AspNetCore或Rougamo.Retry.GeneralHost，那么你同样需要完成这些组件各自的初始化操作
public void ConfigureServices(IServiceCollection services)
{
    services.AddRecordable<Recordable>();
}

// 3. 使用，以下操作都会自动执行Recordable的异常记录动作
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
- `RetryAttribute`和`RecordRetryAttribute`继承的是`MoAttribute`而不是`ExMoAttribute`，所以不推荐将`RetryAttribute`和`RecordRetryAttribute`应用于Task/ValueTask返回值但不是async/await写法的方法，除非你真的知道实际处理逻辑