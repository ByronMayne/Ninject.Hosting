using Microsoft.Extensions.DependencyInjection;
using Ninject;

namespace Ninject.Hosting.Internal
{
    /// <summary>
    /// Factory for initializing a new instance of a 
    /// </summary>
    internal class NinjectServiceProviderFactory : IServiceProviderFactory<NinjectContainerBuilder>
    {
        /// <summary>
        /// Gets the factory used for creating the instance of the kernel
        /// </summary>
        public Func<INinjectSettings, IKernel> KernelFactory { get; }

        /// <summary>
        /// Action to invoke to allow users to change the settings of Ninject
        /// </summary>
        public Action<INinjectSettings> Configure { get; }


        /// <summary>
        /// Creates a new instace that allows the user to choose a custom type of kernel to use
        /// </summary>
        /// <param name="factory">The factory method used to create the kernel</param>
        /// <param name="configure">A callback to allow users to configure the settings of Ninject.</param>
        public NinjectServiceProviderFactory(
            Action<INinjectSettings> configure,
            Func<INinjectSettings, IKernel> factory)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (configure is null) throw new ArgumentNullException(nameof(configure));

            KernelFactory = factory;
            Configure = configure;
        }

        /// <inheritdoc cref="IServiceProviderFactory{TContainerBuilder}"/>
        public NinjectContainerBuilder CreateBuilder(IServiceCollection services)
        {
            return new NinjectContainerBuilder(services, KernelFactory, Configure);
        }

        /// <inheritdoc cref="IServiceProviderFactory{TContainerBuilder}"/>
        public IServiceProvider CreateServiceProvider(NinjectContainerBuilder builder)
        {
            return builder.Build();
        }
    }
}
