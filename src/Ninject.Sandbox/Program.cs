using Microsoft.Extensions.Hosting;

namespace Ninject.Sandbox
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Default 
            new HostBuilder()
                .UseNinject();

            // Settings 
            new HostBuilder()
                .UseNinject(configre =>
                {
                    configre.AllowNullInjection = true;
                });

            // Custom Kernel 
            new HostBuilder()
                .UseNinject(
                    configure: (config) => config.AllowNullInjection = true,
                    factory: (settings) => new StandardKernel(settings));

            Console.WriteLine("Hello, World!");
        }
    }
}
