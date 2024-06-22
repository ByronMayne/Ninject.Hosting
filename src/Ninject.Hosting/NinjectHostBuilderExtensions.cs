using Ninject;
using Ninject.Hosting.Internal;

namespace Microsoft.Extensions.Hosting
{

    /// <summary>
    /// Contains extension methods for working with <see cref="IHostBuilder"/>
    /// </summary>
    public static class NinjectHostBuilderExtensions
    {
        /// <summary>
        /// Replaces the build in Dependency Injection framework with <see cref="IKernel"/> instead. 
        /// </summary>
        /// <param name="hostBuilder">The host builder to replace the framework with</param>
        /// <returns>The host builder</returns>
        public static IHostBuilder UseNinject(this IHostBuilder hostBuilder)
            => UseNinjectInternal(hostBuilder, null, null);

        /// <summary>
        /// Replaces the build in Dependency Injection framework with <see cref="IKernel"/> instead. 
        /// </summary>
        /// <param name="hostBuilder">The host builder to replace the framework with</param>
        /// <param name="factory">A factory method for creating a new <see cref="IKernel"/>. THe default version creates a <see cref="StandardKernel"/>.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        /// <exception cref="ArgumentNullException">The factory method was null</exception>
        public static IHostBuilder UseNinject(this IHostBuilder hostBuilder, Func<INinjectSettings, IKernel> factory)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            return UseNinjectInternal(hostBuilder, null, factory);
        }

        /// <summary>
        /// Replaces the build in Dependency Injection framework with <see cref="IKernel"/> instead. 
        /// </summary>
        /// <param name="hostBuilder">The host builder to replace the framework with</param>
        /// <param name="configure">A method callback to allow you to configure the <see cref="INinjectSettings"/> of the created <see cref="IKernel"/></param>
        /// <returns><see cref="IHostBuilder"/></returns>
        /// <exception cref="ArgumentNullException">The configure callback was null</exception>
        public static IHostBuilder UseNinject(this IHostBuilder hostBuilder, Action<INinjectSettings> configure)
        {
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            return UseNinjectInternal(hostBuilder, configure, null);
        }
        /// <summary>
        /// Replaces the build in Dependency Injection framework with <see cref="IKernel"/> instead. 
        /// </summary>
        /// <param name="hostBuilder">The host builder to replace the framework with</param>
        /// <param name="configure">A method callback to allow you to configure the <see cref="INinjectSettings"/> of the created <see cref="IKernel"/></param>
        /// <param name="factory">A factory method for creating a new <see cref="IKernel"/>. THe default version creates a <see cref="StandardKernel"/>.</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        /// <exception cref="ArgumentNullException">Either the configure or factory callbacks were null.</exception>
        public static IHostBuilder UseNinject(this IHostBuilder hostBuilder, Action<INinjectSettings> configure, Func<INinjectSettings, IKernel> factory)
        {
            if (factory is null) throw new ArgumentNullException(nameof(factory));
            if (configure is null) throw new ArgumentNullException(nameof(configure));
            return UseNinjectInternal(hostBuilder, configure, factory);
        }

        /// <summary>
        /// Internal method for setting up Ninject as the dependency injection provider. This will set default values
        /// for the accepted callbacks.
        /// </summary>
        private static IHostBuilder UseNinjectInternal(IHostBuilder hostBuilder,
               Action<INinjectSettings>? configure,
               Func<INinjectSettings, IKernel>? factory)
        {
            configure ??= ConfigureKernel;
            factory ??= CreateKernel;

            NinjectServiceProviderFactory factoryProvider = new NinjectServiceProviderFactory(configure, factory);
            hostBuilder.UseServiceProviderFactory(factoryProvider);
            return hostBuilder;
        }

        /// <summary>
        /// Default method for configuration the Ninject <see cref="IKernel"/>
        /// </summary>
        /// <param name="settings">The settings object</param>
        private static void ConfigureKernel(INinjectSettings settings)
        { }

        /// <summary>
        /// Default method for creating an instance of <see cref="IKernel"/>
        /// </summary>
        /// <param name="settings">The settings that should be used</param>
        /// <returns>The created kernel</returns>
        private static IKernel CreateKernel(INinjectSettings settings)
            => new StandardKernel(settings);
    }
}
