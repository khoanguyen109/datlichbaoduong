using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace yellowx.Framework.Dependency.Windsor
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public virtual void Install(IWindsorContainer container, IConfigurationStore store)
        {
        }
    }
}
