# Ninject.Hosting 

Allows for replacing of `Microsoft.Extensions.DependencyInjection` with [Ninject](https://www.google.com/search?client=firefox-b-d&q=Ninject+github) by extending the `IHostBuilder` interface.

## Get Started 

To use the library you just need to reference the NuGet package. Then there will be a new extension method added called `UseNinject`. 

```csharp
new HostBuilder()
    .UseNinject()
```

You can also customize the settings

```csharp 
new HostBuilder()
    .UseNinject(configre =>
    {
        configre.AllowNullInjection = true;
    });
```

or create your own type of Ninject `IKernel`

```csharp 
new HostBuilder()
    .UseNinject(
        configure: (config) => 
        {
            config.AllowNullInjection = true;
        } 
        factory: (settings) => 
        {
            return new StandardKernel(settings);
        });
```