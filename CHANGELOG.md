# 5.0

- 将`Rougamo.Fody`升级到 5.0
- 使用`AdviceAttribute`和`OptimizationAttribute`优化`RetryAttribute`和`RecordRetryAttribute`
- `Rougamo.Retry.AspNetCore`和`Rougamo.Retry.GeneralHost`虽然会一同发布 5.0，但 NuGet将标记为 Deprecated，后续依赖注入推荐使用 [DependencyInjection.StaticAccessor](https://github.com/inversionhourglass/DependencyInjection.StaticAccessor)

    ```csharp
    // 修改对象创建方式，从 PinnedScope.ScopedServices 中获取对象实例
    Resolver.Set(t => PinnedScope.ScopedServices!.GetRequiredService(t));

    // 后续初始化请根据你的项目类型，参照 DependencyInjection.StaticAccessor 系列文档完成
    ```

- 为 netstandard2.1 下的`IRecordable`增加了异步异常处理方法`TemporaryFailedAsync`和`UltimatelyFailedAsync`，方便进行异步操作。只为 netstandard2.1 增加异步方法，是因为 netstandard2.1 支持默认接口方法，所以`ISyncRecordable`和`IAsyncRecordable`，两个接口分别实现了默认的异步方法和默认的同步方法，方便使用。同时，由于`IRecordableMatcher`和`IRecordableRetryDefinition`也实现了`IRecordable`接口，所以也为他们增加了实现了默认方法的同步/异步接口。