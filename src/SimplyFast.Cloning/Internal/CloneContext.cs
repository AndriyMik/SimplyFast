﻿using System;
using System.Collections.Generic;
using SimplyFast.Comparers;

namespace SimplyFast.Cloning.Internal
{
    internal class CloneContext: ICloneContext
    {
        private readonly CloneObject _cloneObject;
        private readonly object _cloning = new object();

        private readonly Dictionary<object, object> _objects = new Dictionary<object, object>(EqualityComparerEx.Reference());

        public CloneContext(CloneObject cloneObject)
        {
            _cloneObject = cloneObject;
        }

        public object Clone(object obj)
        {
            if (ReferenceEquals(obj, null))
            {
                return default;
            }
            if (_objects.TryGetValue(obj, out var clonedObj)) {
                if (ReferenceEquals(clonedObj, _cloning))
                    throw new InvalidOperationException("Circular reference found");
                return clonedObj;
            }

            _objects.Add(obj, _cloning);
            
            // clone
            clonedObj = _cloneObject(this, obj);

            _objects[obj] = clonedObj;

            return clonedObj;
        }
    }
}