using System;

namespace MaintenanceSchedule.Library.NHibernate
{
    [AttributeUsage(AttributeTargets.Class)]
    public class SessionAttribute : Attribute
    {
        public readonly string _factoryKey;

        public SessionAttribute(string factoryKey)
        {
            _factoryKey = factoryKey;
        }

        public static string GetFactoryKey(object target)
        {
            Type objectType = target.GetType();

            object[] attributes = objectType.GetCustomAttributes(typeof(SessionAttribute), true);

            if (attributes.Length > 0)
            {
                var attribute = (SessionAttribute)attributes[0];
                return attribute._factoryKey;
            }

            return SessionFactory.DefaultFactoryKey;
        }
    }
}
