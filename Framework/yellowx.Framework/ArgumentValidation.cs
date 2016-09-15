﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using yellowx.Framework.Exceptions;

namespace yellowx.Framework
{
    /// <summary>
    /// Utility for setting method preconditions
    /// </summary>
    public static class ArgumentValidation
    {
        /// <summary>
        /// Ensures an object is not null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void NotNull<T>(T param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Ensures an object is not null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void NotNull<T>(T param)
        {
            if (param == null)
                throw new ArgumentNullException(typeof(T).Name);
        }

        public static void NotNull<T>(T? param, string paramName) where T : struct
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        public static void NotNull(string param, string paramName)
        {
            if (param == null)
                throw new ArgumentNullException(paramName);
        }

        /// <summary>
        /// Ensures a value type is not of its default value.
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void NotDefault<T>(T param, string paramName) where T : struct
        {
            if (param.Equals(default(T)))
                throw new ArgumentException("Parameter cannot be its default value.", paramName);
        }

        /// <summary>
        /// Ensures a string is not null or empty.
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void NotNullOrEmpty(string param, string paramName)
        {
            if (string.IsNullOrEmpty(param))
                throw new ArgumentException("Parameter cannot be a null or empty string.", paramName);
        }

        /// <summary>
        /// Ensures a list is not null or empty.
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void NotEmpty<T>(IEnumerable<T> param, string paramName) where T : class
        {
            NotNull(param, paramName);

            if (!param.Any())
                throw new ArgumentException("Parameter cannot be a empty list.", paramName);
        }

        /// <summary>
        /// Ensures a dependent component is not null
        /// </summary>
        /// <param name="component"></param>
        /// <param name="instanceName"></param>
        /// <exception cref="DependentComponentException{T}" />
        public static void ComponentNotNull<T>(T component, string instanceName) where T : class
        {
            if (component == null)
                throw new DependentComponentException<T>(instanceName);
        }

        /// <summary>
        /// Ensures a dependent component is not null
        /// </summary>
        /// <param name="component"></param>
        /// <exception cref="DependentComponentException{T}" />
        public static void ComponentNotNull<T>(T component) where T : class
        {
            if (component == null)
                throw new DependentComponentException<T>();
        }

        /// <summary>
        /// Ensures an integer (param) is greater than the value parameter
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void GreaterThan(int value, int param, string paramName)
        {
            if (param <= value)
                throw new ArgumentException(string.Format("Parameter {0} must be greater than {1}", paramName, value));
        }

        /// <summary>
        /// Ensures an integer (param) is greater than the value parameter
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void GreaterThanOrEqual(int value, int param, string paramName)
        {
            if (param < value)
                throw new ArgumentException(string.Format("Parameter {0} must be greater than {1}", paramName, value));
        }

        /// <summary>
        /// Ensures an integer (param) is greater than the value parameter
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void GreaterThan(decimal value, decimal param, string paramName)
        {
            if (param <= value)
                throw new ArgumentException(string.Format("Parameter {0} must be greater than {1}", paramName, value));
        }

        /// <summary>
        /// Ensures an integer (param) is greater than the value parameter
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void LessThan(int value, int param, string paramName)
        {
            if (param >= value)
                throw new ArgumentException(string.Format("Parameter {0} must be less than {1}", paramName, value));
        }

        /// <summary>
        /// Ensures an integer (param) is greater than the value parameter
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void LessThan<T>(T value, T param, string paramName) where T : IComparable
        {
            if (param.CompareTo(value) >= 0)
                throw new ArgumentException(string.Format("Parameter {0} must be less than {1}", paramName, value));
        }

        /// <summary>
        /// Ensures an integer (param) is greater than the value parameter
        /// </summary>
        /// <exception cref="ArgumentException" />
        public static void LessThan(decimal value, decimal param, string paramName)
        {
            if (param >= value)
                throw new ArgumentException(string.Format("Parameter {0} must be less than {1}", paramName, value));
        }
    }
}
