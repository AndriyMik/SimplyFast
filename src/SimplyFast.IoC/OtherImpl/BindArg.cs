﻿using System;
using System.Diagnostics.CodeAnalysis;

namespace SimplyFast.IoC
{
    public struct BindArg : IEquatable<BindArg>
    {
        public bool Equals(BindArg other)
        {
            return Type == other.Type && string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is BindArg arg && Equals(arg);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Type != null ? Type.GetHashCode() : 0) * 397) ^ (Name != null ? Name.GetHashCode() : 0);
            }
        }

        public static bool operator ==(BindArg left, BindArg right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BindArg left, BindArg right)
        {
            return !left.Equals(right);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        private BindArg(Type type, object value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            Type = type;
            Name = null;
            Value = value;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public BindArg(Type type, string name, object value)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            Type = type;
            Name = name;
            Value = value;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        private BindArg(string name, object value)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));
            Type = null;
            Name = name;
            Value = value;
        }

        public readonly Type Type;
        public readonly string Name;
        public readonly object Value;

        public override string ToString()
        {
            return $"{Name ?? "?"}:{(Type != null ? Type.FullName : "<any>")}={Value ?? "<null>"}";
        }

        public bool Match(Type type, string name)
        {
            if (Name == null)
                return type.IsAssignableFrom(Type);
            if (Type == null)
                return string.Equals(Name, name);
            return type.IsAssignableFrom(Type) && string.Equals(Name, name);
        }

        public static BindArg Typed<T>(T value)
        {
            return new BindArg(typeof(T), value);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public static BindArg Named(string name, object value)
        {
            return new BindArg(name, value);
        }
    }
}