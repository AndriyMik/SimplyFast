﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using SimplyFast.Cloning.Internal.Deep;
using SimplyFast.Reflection;
using SimplyFast.Reflection.Emit;

namespace SimplyFast.Cloning
{
    public static class CloneObjectEx
    {
        public static readonly CloneObject Ignore = (c,src) => null;
        public static readonly CloneObject Copy = (c, src) => src;
        public static readonly CloneObject CopyArray = (c, src) => ((Array) src).Clone();

        public static bool UseEmit { get; set; } = true;

        public static CloneObject CloneArray(Type elementType)
        {
            return 
                typeof(DeepCloneArray<>)
                .MakeGenericType(elementType)
                .Methods()[0]
                .InvokerAs<CloneObject>();
        }

        [SuppressMessage("ReSharper", "TailRecursiveCall")]
        public static CloneType GetCloneType(Type type)
        {
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
                return CloneType.Copy;
            var nullable = Nullable.GetUnderlyingType(type);
            if (nullable != null)
                return GetCloneType(nullable);

            return GetCloneTypeFromAttribute(type) ?? CloneType.Deep;
        }

        public static CloneType? GetCloneTypeFromAttribute(MemberInfo member)
        {
            var attr = member.GetCustomAttributes<CloneTypeAttribute>().FirstOrDefault();
            return attr?.Type;
        }

        [SuppressMessage("ReSharper", "TailRecursiveCall")]
        public static CloneObject DeepCloneObject(Type type)
        {
            var nullable = Nullable.GetUnderlyingType(type);

            if (UseEmit && EmitEx.Supported)
                return DeepCloneEmit.Build(nullable ?? type);

            if (nullable != null)
                return new DeepCloneStruct(nullable).Clone;

            return  type.IsValueType ? (CloneObject)new DeepCloneStruct(type).Clone : new DeepCloneClass(type).Clone;
        }
    }
}