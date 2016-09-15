using System;
using System.Collections.Generic;
using System.Linq;

namespace yellowx.Framework.Extensions
{
    public static class GenericExtensions
    {
        public static T IsNotNull<T>(this object o, Func<T> trueClause, Func<T> falseClause)
        {
            if (o != null)
                return trueClause();
            return falseClause();
        }

        public static T IsNotNull<T>(this object o, Func<T> trueClause)
        {
            return o.IsNotNull(trueClause, () => default(T));
        }

        public static void IsNotNull(this object o, Action action)
        {
            if (o != null)
                action();
        }

        public static void IsNotNull(this object o, Action trueAction, Action falseAction)
        {
            if (o != null)
                trueAction();
            else
                falseAction();
        }

        public static void TryCatch(this object o, Action action)
        {
            o.TryCatch(action, null);
        }

        public static void TryCatch(this object o, Action action, Action<System.Exception> actionInCatch)
        {
            try
            {
                action();
            }
            catch (System.Exception ex)
            {
                if (actionInCatch == null)
                    actionInCatch = (e) => Configuration.WriteEventLogEntry("TryCatch Error", e);
                actionInCatch(ex);
            }
        }

        public static T TryCatch<T>(this object o, Func<T> func)
        {
            return o.TryCatch(func, null);
        }

        public static T TryCatch<T>(this object o, Func<T> func, Func<System.Exception, T> funcInCatch)
        {
            try
            {
                return func();
            }
            catch (System.Exception ex)
            {
                if (funcInCatch != null)
                    return funcInCatch(ex);
                else
                    Configuration.WriteEventLogEntry("Default Error", ex);
            }
            return default(T);
        }

        public static bool IsIn<T>(this T obj, params T[] candidates)
        {
            return candidates.Any(c => c == null ? obj == null : c.Equals(obj));
        }

        public static bool IsIn<T>(this T obj, IEnumerable<T> candidates)
        {
            return candidates.Any(c => c == null ? obj == null : c.Equals(obj));
        }

        public static bool NotIn<T>(this T source, params T[] candidates)
        {
            return !candidates.Any(c => c == null ? source == null : c.Equals(source));
        }

        public static T Or<T>(this T source, T alternateIfSourceIsDefault)
        {
            return source.Equals(default(T)) ? alternateIfSourceIsDefault : source;
        }

        public static TOut IfPoss<T, TOut>(this T? nullable, Func<T, TOut> getter, TOut valueIfNotPoss = default(TOut)) where T : struct
        {
            return nullable.Cond(t => !t.HasValue, t => valueIfNotPoss, t => getter(t.Value));
        }

        public static TOut IfPoss<T, TOut>(this T obj, Func<T, TOut> getter, TOut valueIfNotPoss = default(TOut))
            where T : class
        {
            return obj.Cond(t => t == null, t => valueIfNotPoss, getter);
        }

        public static string IfNotNullOrEmpty(this string obj, Func<string, string> getter, string valueIfNotPoss = null)
        {
            return !obj.IsNullOrEmpty() ? getter(obj) : valueIfNotPoss;
        }

        public static T Modify<T>(this T obj, Action<T> modifier)
        {
            modifier(obj);
            return obj;
        }

        public static TOut Use<T, TOut>(this T obj, Func<T, TOut> usage)
        {
            return usage(obj);
        }

        public static TOut Cond<T, TOut>(this T obj, Func<T, bool> test, Func<T, TOut> resultIf, Func<T, TOut> resultElse)
        {
            return test(obj) ? resultIf(obj) : resultElse(obj);
        }

        public static T? AsNullable<T>(this T t) where T : struct
        {
            return t;
        }
    }
}
