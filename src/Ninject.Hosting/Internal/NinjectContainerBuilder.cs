using Microsoft.Extensions.DependencyInjection;
using Ninject.Activation;
using Ninject.Planning.Targets;
using Ninject.Syntax;
using System.Reflection;

namespace Ninject.Hosting.Internal
{
    internal class NinjectContainerBuilder
    {
        private readonly IServiceCollection m_services;
        private readonly Func<INinjectSettings, IKernel> m_kernelFactory;
        private readonly Action<INinjectSettings> m_configure;

        public NinjectContainerBuilder(IServiceCollection services, Func<INinjectSettings, IKernel> kernelFactory, Action<INinjectSettings> configure)
        {
            m_services = services;
            m_kernelFactory = kernelFactory;
            m_configure = configure;
        }

        /// <summary>
        /// Creates the new instance of the <see cref="IKernel"/>
        /// </summary>
        /// <returns>The created kernel</returns>
        public IKernel Build()
        {
            NinjectSettings settings = new();
            try
            {
                m_configure(settings);
            }
            catch (Exception exception)
            {
                throw new Exception("An unhandle exception was thrown while configure the Ninject settings", exception);
            }

            IKernel kernel;
            try
            {
                kernel = m_kernelFactory(settings);
            }
            catch (Exception exception)
            {
                throw new Exception("An unhandled exception was thrown while creating the Ninject kernel", exception);
            }

            foreach (ServiceDescriptor service in m_services)
            {
                IBindingToSyntax<object> bindingSyntax = kernel.Bind(service.ServiceType);
                IBindingInSyntax<object>? toBindingSyntax = BindTo(service, bindingSyntax);

                if (toBindingSyntax is null)
                {
                    continue;
                }

                switch (service.Lifetime)
                {
                    case ServiceLifetime.Transient:
                        _ = toBindingSyntax.InTransientScope();
                        break;
                    case ServiceLifetime.Singleton:
                        _ = toBindingSyntax.InSingletonScope();
                        break;
                    case ServiceLifetime.Scoped:
                        _ = toBindingSyntax.InTransientScope(); // Not sure what to do here since we are not tied to asp.net core 
                        break;
                }
            }

            return kernel;
        }

        private static IBindingInSyntax<object>? BindTo(ServiceDescriptor service, IBindingToSyntax<object> nameSyntax)
        {
            IBindingWhenInNamedWithOrOnSyntax<object>? namedBinding = BuildImplementionBinding(service, nameSyntax);
            if (namedBinding is null)
            {
                return null;
            }

            return service.IsKeyedService ? namedBinding.When(r => IsKeyedMember(r, service)) : namedBinding;
        }

        private static IBindingWhenInNamedWithOrOnSyntax<object>? BuildImplementionBinding(ServiceDescriptor service, IBindingToSyntax<object> nameSyntax)
        {
            if (service.ImplementationInstance != null)
            {
                return nameSyntax.ToConstant(service.ImplementationInstance);
            }
            else if (service.ImplementationFactory != null)
            {
                return nameSyntax.ToMethod(c => service.ImplementationFactory(c.Kernel));
            }
            else
            {
                return service.ImplementationType != null ? nameSyntax.To(service.ImplementationType) : null;
            }
        }

        /// <summary>
        /// Adds a condition that this binding will only be resovled if it has <see cref="FromKeyedServicesAttribute"/>
        /// with a value that matches the <see cref="ServiceDescriptor.ServiceKey"/> value.
        /// </summary>
        /// <param name="request">The request condition</param>
        /// <param name="service">The service descriptor</param>
        /// <returns>The modfied collection</returns>
        private static bool IsKeyedMember(IRequest request, ServiceDescriptor service)
        {
            if (request.Target is null)
            {
                return false;
            }

            ITarget target = request.Target;
            MemberInfo memberInfo = target.Member;
            FromKeyedServicesAttribute? attribute = memberInfo.GetCustomAttribute<FromKeyedServicesAttribute>();
            if (attribute is null)
            {
                return false;
            }

            object key = attribute.Key;
            return object.Equals(key, service.ServiceKey);

        }
    }
}
