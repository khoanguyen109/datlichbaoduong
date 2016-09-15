using System;
using System.Reflection;
using System.Linq;
using yellowx.Framework.Extensions;

namespace yellowx.Framework.Dependency
{
    public interface IObjectFactory
    {
        T[] ResolveAll<T>();
        T Resolve<T>();
        object Resolve(Type type);
        object[] ResolveAll(Type type);
        bool CanResolve(Type type);
        bool CanResolve<T>();
        void Release(object o);
    }

    public interface IObjectFactory<TContainer> : IObjectFactory
    {
        TContainer Container { get; }
    }

    public abstract class ObjectFactory<TContainer> : Object, IObjectFactory<TContainer>
    {
        protected TContainer _container;
        public virtual TContainer Container
        {
            get
            {
                return _container;
            }
        }

        public abstract T[] ResolveAll<T>();
        public abstract T Resolve<T>();
        public abstract object Resolve(Type type);
        public abstract object[] ResolveAll(Type type);
        public abstract void Release(object o);
        public abstract bool CanResolve(Type type);
        public virtual bool CanResolve<T>()
        {
            return CanResolve(typeof(T));
        }
    }
}
