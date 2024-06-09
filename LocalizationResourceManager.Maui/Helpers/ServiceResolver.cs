namespace LocalizationResourceManager.Maui.Helpers;

/// <summary>
/// Store and access a service provider to resolve specific types.
/// </summary>
public static class ServiceResolver
{
    private static IServiceProvider? _serviceProvider;
    public static IServiceProvider ServiceProvider => _serviceProvider ?? throw new Exception("Service provider has not been initialized");

    /// <summary>
    /// Register the service provider
    /// </summary>
    public static void RegisterServiceProvider(IServiceProvider sp)
    {
        _serviceProvider = sp;
    }

    /// <summary>
    /// Get service of type <typeparamref name="T"/> from the service provider.
    /// </summary>
    public static T Resolve<T>() where T : class => ServiceProvider.GetRequiredService<T>();

    /// <summary>
    /// Attempt top get service of type <typeparamref name="T"/> from the service provider.
    /// </summary>
    /// <typeparam name="T">Service type to resolve.</typeparam>
    /// <param name="service">Instance of resolved service of type <typeparamref name="T"/>.</param>
    /// <returns>Flag indicating if service type was resolved.</returns>
    public static bool TryResolve<T>(out T? service) where T : class
    {
        service = ServiceProvider.GetService<T>();
        return service is not null;
    }

    /// <summary>
    /// <see cref="IServiceProvider"/> extension method to register provider in resolver.
    /// </summary>
    /// <param name="sp"></param>
    public static void UseResolver(this IServiceProvider sp)
    {
        RegisterServiceProvider(sp);
    }
}