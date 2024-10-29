namespace LocalizationResourceManager.Maui;

/// <summary>
/// Interface for Micro Container
/// </summary>
internal interface IMicroContainer
{
    /// <summary>
    /// Check if service is registered.
    /// </summary>
    /// <typeparam name="TService">Type of service to check</typeparam>
    /// <returns>Flag indication if service is registered</returns>
    bool Contains<TService>();

    /// <summary>
    /// Get service from container.
    /// </summary>
    /// <typeparam name="TService">Type of service to get</typeparam>
    /// <returns>Service or default</returns>
    TService? Get<TService>();

    /// <summary>
    /// Add service to container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add</typeparam>
    /// <param name="service">Service to add</param>
    /// <returns>Flag indication if service was added successfully</returns>
    bool Add<TService>(TService service);

    /// <summary>
    /// Add service factory to container.
    /// </summary>
    /// <typeparam name="TService">Type of service to add</typeparam>
    /// <param name="factory">Factory to create service</param>
    /// <returns>Flag indication if service factory was added successfully</returns>
    bool Add<TService>(Func<TService> factory);

    /// <summary>
    /// Remove service from container.
    /// </summary>
    /// <typeparam name="TService">Type of service to remove</typeparam>
    /// <returns>Flag indication if service was removed successfully</returns>
    bool Remove<TService>();
}