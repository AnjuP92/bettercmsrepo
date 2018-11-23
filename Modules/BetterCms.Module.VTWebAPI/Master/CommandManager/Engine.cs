using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BetterCms.Module.VTWebAPI.Master
{
    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Activators.Delegate;
    using Autofac.Core.Lifetime;
    using Autofac.Core.Registration;

    public class HandlerRegistrationSource : IRegistrationSource
    {
        public IEnumerable<IComponentRegistration> RegistrationsFor(
          Service service,
          Func<Service, IEnumerable<IComponentRegistration>> registrationAccessor)
        {
            var swt = service as IServiceWithType;
            if (swt == null || !typeof(BaseCommandHandler).IsAssignableFrom(swt.ServiceType))
            {
                // It's not a request for the base handler type, so skip it.
                return Enumerable.Empty<IComponentRegistration>();
            }

            // This is where the magic happens!
            var registration = new ComponentRegistration(
              Guid.NewGuid(),
              new DelegateActivator(swt.ServiceType, (c, p) =>
              {
                  // In this example, the factory itself is assumed to be registered
                  // with Autofac, so we can resolve the factory. If you want to hard
                  // code the factory here, you can do that, too.
                  var provider = c.Resolve<IHandlerFactory>();

                  // Our factory interface is generic, so we have to use a bit of
                  // reflection to make the call.
                  var method = provider.GetType().GetMethod("GetHandler").MakeGenericMethod(swt.ServiceType);

                  // In the end, return the object from the factory.
                  return method.Invoke(provider, null);
              }),
              new CurrentScopeLifetime(),
              InstanceSharing.None,
              InstanceOwnership.OwnedByLifetimeScope,
              new[] { service },
              new Dictionary<string, object>());

            return new IComponentRegistration[] { registration };
        }

        public bool IsAdapterForIndividualComponents { get { return false; } }
    }


    public abstract class BaseCommandHandler
    {
       // public IComm
        public virtual string ExecuteCommand(string message)
        {
            return "Handled: " + message;
        }
    }

    public interface IHandlerFactory
    {
        T GetHandler<T>() where T : BaseCommandHandler;
    }

    public class HandlerFactory : IHandlerFactory
    {
        public T GetHandler<T>() where T : BaseCommandHandler
        {
            return (T)Activator.CreateInstance(typeof(T));
        }
    }
}