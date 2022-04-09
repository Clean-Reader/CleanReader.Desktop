// Copyright (c) Richasy. All rights reserved.

using Microsoft.Extensions.DependencyInjection;

namespace CleanReader.Locator.Lib
{
    /// <summary>
    /// Service locator, used to obtain the container for dependency injection.
    /// </summary>
    public class ServiceLocator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceLocator"/> class.
        /// </summary>
        /// <param name="serviceCollection">Service provider instance.</param>
        public ServiceLocator(ServiceCollection serviceCollection)
        {
            this.ServiceCollection = serviceCollection;
            this.ServiceProvider = serviceCollection.BuildServiceProvider();
            Instance = this;
        }

        /// <summary>
        /// Instance of <see cref="ServiceLocator"/>.
        /// </summary>
        public static ServiceLocator Instance { get; private set; }

        /// <summary>
        /// Service provider instance.
        /// </summary>
        public ServiceProvider ServiceProvider { get; set; }

        /// <summary>
        /// Service collection instance.
        /// </summary>
        public ServiceCollection ServiceCollection { get; }

        /// <summary>
        /// Get registered service.
        /// </summary>
        /// <typeparam name="T">Service registration type.</typeparam>
        /// <returns>Service.</returns>
        public T GetService<T>()
            => ServiceProvider != null ? ServiceProvider.GetService<T>() : default;

        /// <summary>
        /// Get registered service.
        /// </summary>
        /// <param name="typeName">Service type name.</param>
        /// <typeparam name="T">Service registration type.</typeparam>
        /// <returns>Service.</returns>
        public T GetService<T>(string typeName)
            => ServiceProvider != null ? ServiceProvider.GetServices<T>().FirstOrDefault(p => p.GetType().Name == typeName) : default;

        /// <summary>
        /// Try to load the service.
        /// </summary>
        /// <typeparam name="T">Service registration type.</typeparam>
        /// <param name="defineService">Definition of need to load the service.</param>
        /// <returns>Whether the loading is successful.</returns>
        public ServiceLocator LoadService<T>(out T defineService)
        {
            if (ServiceProvider == null)
            {
                defineService = default;
            }
            else
            {
                var service = GetService<T>();
                defineService = service;
            }

            return this;
        }
    }
}
