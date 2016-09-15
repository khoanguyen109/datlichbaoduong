using Castle.MicroKernel.Context;
using Castle.MicroKernel.Lifestyle.Scoped;
using System.Web;

namespace yellowx.Framework.Dependency.Windsor
{
    /// <summary>
    /// Custom LifeStyle to automatically detect the context in which the application is being used and return appropriate scope for the objects. 
    /// Following is the order in which scopes are detected and the first one found is returned: 
    /// - WCF Operation 
    /// - Web Request  
    /// - Transient
    /// </summary>
    /// No new scope is implemented, it just  provides a Hybrid model for scopes provided by Castle Windsor and WCF Integration Factility. 
    /// Primarily introduced so that the Data Access code using NHibernate Sessions can work within WCF Operation as well as Web Request. 
    /// Following configuration must be added to web.config for the WebRequest Lifetime Scope to work: 
    /// <configuration>  
    ///     <system.webServer>  
    ///         <modules>   
    ///             <add name="PerRequestLifestyle" type="Castle.MicroKernel.Lifestyle.PerWebRequestLifestyleModule, Castle.Windsor" />
    ///         </modules>
    /// </system.webServer> 
    /// </configuration>  
    /// Instead of avoiding using <modules></modules> in system.webServer, we should implement a private IDependencyScope to BeginScope and IDependencyResolver
    /// to resolve request.
    ///     public class WindsorDependencyScope : IDependencyScope{ }   
    ///     public class WindsorDependencyResolver : IDependencyResolver
    /// 
    public class WindsorHybridScopeAccessor : IScopeAccessor
    {
        //private readonly IScopeAccessor wcfOperationScopeAccessor = new Castle.Facilities.WcfIntegration.Lifestyles.WcfOperationScopeAccessor();
        private readonly IScopeAccessor webRequestScopeAccessor = new Castle.MicroKernel.Lifestyle.WebRequestScopeAccessor();
        private readonly IScopeAccessor callContextScopeAccessor = new CallContextScopeAccessor();


        public ILifetimeScope GetScope(CreationContext context)
        {
            //if (OperationContext.Current != null)
            //return wcfOperationScopeAccessor.GetScope(context);
            if (HttpContext.Current != null)
                return webRequestScopeAccessor.GetScope(context);
            else
                return callContextScopeAccessor.GetScope(context);
        }

        #region IDisposable
        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    callContextScopeAccessor.Dispose();
                    webRequestScopeAccessor.Dispose();
                }
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion


    }
    /// <summary>
    /// Used by the HybridLifeStyleScopeAccessor class to provide the Transient LifeStyle as fallback
    /// </summary>
    public class CallContextScopeAccessor : IScopeAccessor
    {
        public ILifetimeScope GetScope(CreationContext context)
        {
            return new DefaultLifetimeScope();
            //return CallContextLifetimeScope.ObtainCurrentScope();
        }

        #region IDisposable

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                var scope = CallContextLifetimeScope.ObtainCurrentScope();
                if (scope != null)
                    scope.Dispose();
            }
        }

        public void Dispose()
        {

            Dispose(true);
        }
        #endregion
    }
}
