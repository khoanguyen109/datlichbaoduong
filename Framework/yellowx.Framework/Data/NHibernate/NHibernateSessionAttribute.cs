using System;

namespace yellowx.Framework.Data.NHibernate
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NHibernateSessionAttribute : Attribute
    {
        public readonly string factoryKey;

        public NHibernateSessionAttribute(string factoryKey)
        {
            this.factoryKey = factoryKey;
        }
        public static string GetFactoryKey(object target)
        {
            Type objectType = target.GetType();

            object[] attributes = objectType.GetCustomAttributes(typeof(NHibernateSessionAttribute), true);

            if (attributes.Length > 0)
            {
                var attribute = (NHibernateSessionAttribute)attributes[0];
                return attribute.factoryKey;
            }

            return NHibernateSessionFactory.DefaultFactoryKey;
        }
    }
}
