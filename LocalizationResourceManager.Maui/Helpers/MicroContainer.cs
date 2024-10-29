namespace LocalizationResourceManager.Maui.Helpers;

/// <summary>
/// Implementation of Micro Container Interface
/// </summary>
internal class MicroContainer : IMicroContainer
{
    private readonly Dictionary<Type, object> container = [];

    /// <summary>
    /// Check if service is registered.
    /// </summary>
    /// <typeparam name="TService">Type of service to check</typeparam>
    /// <returns>Flag indication if service is registered</returns>
    public bool Contains<TService>() => container.ContainsKey(typeof(TService));

    /// <summary>
    /// Get service from container.
    /// </summary>
    /// <typeparam name="TService">Type of service to get</typeparam>
    /// <remarks>If service is registered with a service factory, the facory is invoked.</remarks>
    /// <returns>Service or default</returns>
    public TService? Get<TService>()
    {
        //Attempt to get service
        if (container.TryGetValue(typeof(TService), out var service))
        {
            //Check if service is TService or a factory
            if (service is Func<TService> factory)
            {
                //Create Service
                service = factory.Invoke();
            }

            //Check if service is TService
            if (service is TService typedService)
            {
                //Return Service
                return typedService;
            }
        }

        //Return default
        return default;
    }

    /// <summary>
    /// Add service to container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add</typeparam>
    /// <param name="service">Service to add</param>
    /// <returns>Flag indication if service was added successfully</returns>
    public bool Add<TService>(TService service)
    {
        //Verify parameters
        ArgumentNullException.ThrowIfNull(service);

        //Add Service
        return container.TryAdd(typeof(TService), service);
    }

    /// <summary>
    /// Add service factory to container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add</typeparam>
    /// <param name="factory">Factory to create service</param>
    /// <returns>Flag indication if service factory was added successfully</returns>
    public bool Add<TService>(Func<TService> factory)
    {
        //Verify parameters
        ArgumentNullException.ThrowIfNull(factory);

        //Add Service Factory
        return container.TryAdd(typeof(TService), factory);
    }

    /// <summary>
    /// Remove service from container.
    /// </summary>
    /// <typeparam name="TService">Type of service to remove</typeparam>
    /// <returns>Flag indication if service was removed successfully</returns>
    public bool Remove<TService>()
    {
        //Remove Service
        return container.Remove(typeof(TService));
    }
}