﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using SimplyFast.Cache;
using SimplyFast.Comparers;

namespace SimplyFast.Reflection.Internal
{
    internal class ConstructorInfoCache
    {
        private static readonly ICache<Type, ConstructorInfoCache> _constructorCache = CacheEx.ThreadSafe<Type, ConstructorInfoCache>();

        // ReSharper disable MemberHidesStaticFromOuterClass
        public readonly ConstructorInfo[] Constructors;
        // ReSharper restore MemberHidesStaticFromOuterClass
        private readonly Dictionary<Type[], ConstructorInfo> _constructors;

        private ConstructorInfoCache(Type type)
        {
            Constructors = type.AllConstructors();
            _constructors = new Dictionary<Type[], ConstructorInfo>(EqualityComparerEx.Array<Type>());
            foreach (var constructorInfo in Constructors)
            {
                _constructors[constructorInfo.GetParameters().Select(x => x.ParameterType).ToArray()] = constructorInfo;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConstructorInfoCache ForType(Type type)
        {
            return _constructorCache.GetOrAdd(type, t => new ConstructorInfoCache(t));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ConstructorInfo Get(Type[] types)
        {
            ConstructorInfo result;
            _constructors.TryGetValue(types, out result);
            return result;
        }
    }
}