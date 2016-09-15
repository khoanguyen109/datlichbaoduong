using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using System;
using System.Linq;

namespace yellowx.Framework.Dependency.Windsor
{
    public class WindsorObjectFactory : ObjectFactory<IWindsorContainer>
    {
        public WindsorObjectFactory()
        {
            try
            {
                //create a windsor container with a specified section 'castle' in configuration file.
                _container = new WindsorContainer(new XmlInterpreter());
                Configuration.WriteDebugView("Create new windsor container.");
            }
            catch (System.Exception ex)
            {
                Configuration.WriteDebugView("Unable to create new windsor container with 'castle' in configuration file.");
                if (ex.Message == "Could not find section 'castle' in the configuration file associated with this domain.")
                {
                    _container = new WindsorContainer();
                    Configuration.WriteDebugView("Create new windsor container without 'castle' in configuration file");
                }
                else
                    throw;
            }
            _container.Kernel.Resolver.AddSubResolver(new ArrayResolver(_container.Kernel));
            _container.Kernel.Resolver.AddSubResolver(new CollectionResolver(_container.Kernel, true));
            //_container.AddFacility<FactorySupportFacility>();
        }

        public override T[] ResolveAll<T>()
        {
            if (CanResolve<T>())
                return _container.ResolveAll<T>();

            return Empty<T>();
        }

        public override T Resolve<T>()
        {
            if(CanResolve<T>())
                return _container.Resolve<T>();

            return Default<T>();
        }

        public override object Resolve(Type type)
        {
            if (CanResolve(type))
                return _container.Kernel.Resolve(type);

            return null;
        }

        public override object[] ResolveAll(Type type)
        {
            if (CanResolve(type))
                return _container.ResolveAll(type).Cast<object>().ToArray();

            return Empty<object>();
        }

        /// <summary>
        /// Uses the type to check if an type of object is registered with the IoC container
        /// </summary>
        /// <returns></returns>
        public override bool CanResolve<T>()
        {
            return CanResolve(typeof(T));
        }

        /// <summary>
        /// Uses the type to check if an object is registered with the IoC container
        /// </summary>
        public override bool CanResolve(Type type)
        {
            return _container.Kernel.HasComponent(type);
        }

        /// <summary>
        /// Release an object from the container
        /// </summary>
        /// <param name="component"></param>
        public override void Release(object component)
        {
            _container.Release(component);
        }
    }
}
