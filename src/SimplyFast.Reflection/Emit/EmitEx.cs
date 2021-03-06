﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SimplyFast.Reflection.Emit
{
    public static class EmitEx
    {
        private static readonly object _lock = new object();
        private static volatile bool _supportedChecked;
        private static volatile bool _supported;

        public static bool Supported
        {
            get
            {
                if (_supportedChecked)
                    return _supported;
                lock (_lock)
                {
                    if (_supportedChecked)
                        return _supported;
                    _supported = CheckEmitSupport();
                    _supportedChecked = true;
                    return _supported;
                }
            }
        }

        private static bool CheckEmitSupport()
        {
            try
            {
                var m = new DynamicMethod(string.Empty, typeof(void), Type.EmptyTypes);
                return m.ReturnType == typeof(void);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Creates dynamic method that matches delegateType
        /// </summary>
        public static DynamicMethod CreateMethod(this Module m, Type delegateType, string name, bool skipVisibility = false)
        {
            var invoke = MethodInfoEx.GetInvokeMethod(delegateType);
            return new DynamicMethod(name, invoke.ReturnType, invoke.GetParameterTypes(), m, skipVisibility);
        }

        /// <summary>
        /// Creates dynamic method that matches delegateType
        /// </summary>
        public static DynamicMethod CreateMethod(this Type owner, Type delegateType, string name = null, bool skipVisibility = false)
        {
            var invoke = MethodInfoEx.GetInvokeMethod(delegateType);
            return new DynamicMethod(name ?? string.Empty, invoke.ReturnType, invoke.GetParameterTypes(), owner, skipVisibility);
        }

        /// <summary>
        /// Creates dynamic method that matches delegateType
        /// </summary>
        public static DynamicMethod CreateMethod<TDelegate>(this Type owner, string name = null, bool skipVisibility = false)
            where TDelegate : class
        {
            return owner.CreateMethod(typeof(TDelegate), name, skipVisibility);
        }

        /// <summary>
        /// Creates dynamic method that matches delegateType
        /// </summary>
        public static DynamicMethod CreateMethod(Type delegateType, string name = null, bool skipVisibility = false)
        {
            return typeof(EmitEx).CreateMethod(delegateType, name, skipVisibility);
        }

        /// <summary>
        /// Creates dynamic method that matches delegateType
        /// </summary>
        public static DynamicMethod CreateMethod<TDelegate>(string name = null, bool skipVisibility = false)
            where TDelegate : class
        {
            return typeof(EmitEx).CreateMethod(typeof(TDelegate), name, skipVisibility);
        }

        public static T CreateDelegate<T>(this DynamicMethod method, object target = null)
            where T : class
        {
            return method.CreateDelegate(typeof(T), target) as T;
        }
    }
}
