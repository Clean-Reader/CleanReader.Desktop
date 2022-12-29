// Copyright (c) Richasy. All rights reserved.

using Autofac;

namespace CleanReader.Locator.Lib;

/// <summary>
/// Service locator.
/// </summary>
public sealed class Locator
{
    private readonly ContainerBuilder _containerBuilder;
    private IContainer _container;

    private Locator()
        => _containerBuilder = new ContainerBuilder();

    /// <summary>
    /// Dependency injection container instance.
    /// </summary>
    public static Locator Instance { get; } = new Lazy<Locator>(() => new Locator()).Value;

    /// <summary>
    /// Registered singletons that provide interfaces and implementations.
    /// </summary>
    /// <typeparam name="TInterface">Singleton interface.</typeparam>
    /// <typeparam name="TImplementation">Singleton implementation.</typeparam>
    /// <returns>Service locator.</returns>
    public Locator RegisterSingleton<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _containerBuilder.RegisterType<TImplementation>()
            .As<TInterface>()
            .SingleInstance();
        return this;
    }

    /// <summary>
    /// Registered singletons that provide instance.
    /// </summary>
    /// <param name="implementation">Instance.</param>
    /// <typeparam name="TInterface">Singleton interface.</typeparam>
    /// <returns>Service locator.</returns>
    public Locator RegisterSingleton<TInterface>(TInterface implementation)
        where TInterface : class
    {
        _containerBuilder.RegisterType<TInterface>()
            .AsSelf()
            .SingleInstance();
        return this;
    }

    /// <summary>
    /// Register constant.
    /// </summary>
    /// <param name="data">Constant data.</param>
    /// <typeparam name="TInterface">Constant type.</typeparam>
    /// <returns>Service locator.</returns>
    public Locator RegisterConstant<TInterface>(TInterface data)
        where TInterface : class
    {
        _containerBuilder.RegisterInstance(data);
        return this;
    }

    /// <summary>
    /// Register transient type.
    /// </summary>
    /// <typeparam name="TInterface">Transient interface.</typeparam>
    /// <typeparam name="TImplementation">Transient implementation.</typeparam>
    /// <returns>Service locator.</returns>
    public Locator RegisterTransient<TInterface, TImplementation>()
        where TInterface : class
        where TImplementation : class, TInterface
    {
        _containerBuilder.RegisterType<TImplementation>()
            .As<TInterface>();
        return this;
    }

    /// <summary>
    /// Build a service provider to enable the registered service to take effect.
    /// </summary>
    public void Build()
        => _container = _containerBuilder.Build();

    /// <summary>
    /// Get the registered service.
    /// </summary>
    /// <typeparam name="T">The interface for service registration.</typeparam>
    /// <returns>The registered service instance.</returns>
    public T GetService<T>()
        => _container.Resolve<T>();
}
