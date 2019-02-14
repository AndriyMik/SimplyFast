﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace SimplyFast.Cloning.Tests
{
    public class CloneTests
    {
        [Fact]
        public void EverythingWorksOk()
        {
            var testInner = new NormalClass(1, null, new CopyClass(), new IgnoreClass());
            var testOuter = new NormalClass(2, testInner, new CopyClass(), new IgnoreClass());

            var outerClone = Clone.Deep(testOuter);
            var innerClone = outerClone.Test;

            Assert.NotNull(innerClone);
            Assert.False(ReferenceEquals(innerClone, testInner));
            Assert.Equal(1, innerClone.Prop);
            Assert.Null(innerClone.Test);
            Assert.Null(innerClone.ReadonlyTest);

            Assert.True(ReferenceEquals(testInner.Copy, innerClone.Copy));
            Assert.Null(innerClone.CopyProp);

            Assert.Null(innerClone.Ignore);
            Assert.Null(innerClone.IgnoreProp);
            Assert.Null(innerClone.IgnoreCopy);

            Assert.Equal(2, outerClone.Prop);
            Assert.NotNull(outerClone.ReadonlyTest);
            Assert.True(ReferenceEquals(outerClone.Test, outerClone.ReadonlyTest));
            //Assert.AreEqual(1, outerClone.ReadonlyTest.Prop);
            Assert.True(ReferenceEquals(testOuter.Copy, outerClone.Copy));
            Assert.True(ReferenceEquals(testOuter.CopyProp, outerClone.CopyProp));

            Assert.Null(outerClone.Ignore);
            Assert.Null(outerClone.IgnoreProp);
            Assert.Null(outerClone.IgnoreCopy);
        }

        [Fact]
        public void ThrowsForCircular()
        {
            var test1 = new NormalClass(1, null, new CopyClass(), new IgnoreClass());
            var test2 = new NormalClass(2, test1, new CopyClass(), new IgnoreClass());
            test1.Test = test2;

            Assert.ThrowsAny<Exception>(() => Clone.Deep(test1));
            Assert.ThrowsAny<Exception>(() => Clone.Deep(test2));
        }

        [Fact]
        public void WorksForReadonly()
        {
            var a = new {test = "t"};
            var b = Clone.Deep(a);
            Assert.NotSame(a, b);
            Assert.Equal(a.test, b.test);
        }

        [Fact]
        public void WorksForStructs()
        {
            var a = new { test = new KeyValuePair<object, int>(Tuple.Create("test", 1), 1) };
            var b = Clone.Deep(a);
            Assert.Equal(Tuple.Create("test", 1), b.test.Key);
            //Assert.IsFalse(ReferenceEquals(a.test.Key, b.test.Key));
            Assert.Equal(1, b.test.Value);
        }

        [Fact]
        public void WorksForArrays()
        {
            var a = new
            {
                list = new[]
                {
                    Tuple.Create("test1", 1),
                    Tuple.Create("test2", 2)
                }
            };
            var b = Clone.Deep(a);
            Assert.False(ReferenceEquals(a.list, b.list));
            Assert.Equal(2, b.list.Length);
            Assert.Equal(Tuple.Create("test1", 1), b.list[0]);
            Assert.Equal(Tuple.Create("test2", 2), b.list[1]);
            Assert.False(ReferenceEquals(a.list[0], b.list[0]));
            Assert.False(ReferenceEquals(a.list[1], b.list[1]));
        }

        [Fact]
        public void WorksForArrayCopy()
        {
            var a = new[] { new CopyClass(), new CopyClass() };
            var b = Clone.Deep(a);
            Assert.Equal(2, b.Length);
            Assert.True(ReferenceEquals(a[0], b[0]));
            Assert.True(ReferenceEquals(a[1], b[1]));
        }

        [Fact]
        public void WorksForArrayIgnore()
        {
            var a = new[] { new IgnoreClass(), new IgnoreClass() };
            var b = Clone.Deep(a);
            Assert.Null(b);
        }


        [Fact]
        public void WorksForArrayMixed()
        {
            var a = new object[] { new NormalClass(2, null, null, null), new CopyClass(), new IgnoreClass() };
            var b = Clone.Deep(a);
            Assert.Equal(3, b.Length);
            Assert.False(ReferenceEquals(a[0], b[0]));
            Assert.Equal(2, ((NormalClass)b[0]).Prop);
            Assert.True(ReferenceEquals(a[1], b[1]));
            Assert.Null(b[2]);
        }

        [Fact]
        public void WeirdEqualClones()
        {
            var parent = new WeirdEqual(null);
            var child = new WeirdEqual(parent);

            var cloned = Clone.Deep(child);

            Assert.NotNull(cloned);
            Assert.NotNull(cloned.Parent);
            Assert.Null(cloned.Parent.Parent);

            Assert.NotSame(child, cloned);
            Assert.NotSame(parent, cloned.Parent);
        }


        [Fact]
        public void NullableOk()
        {
            var s1 = new SomeStruct(5, new object());
            var c1 = Clone.Deep((SomeStruct?)s1);
            Assert.True(c1.HasValue);
            Assert.Equal(5, c1.Value.Int);
            Assert.NotNull(c1.Value.Obj);
            Assert.NotSame(s1.Obj, c1.Value.Obj);

            var cloneNull = Clone.Deep((SomeStruct?)null);
            Assert.False(cloneNull.HasValue);

            var s2 = new SomeStruct(null, s1);
            var c2 = Clone.Deep((SomeStruct?) s2);
            Assert.True(c2.HasValue);
            Assert.False(c2.Value.Int.HasValue);
            Assert.NotNull(c2.Value.Obj);
            Assert.IsType<SomeStruct>(c2.Value.Obj);
            var inner2 = (SomeStruct) c2.Value.Obj;
            Assert.Equal(5, inner2.Int);
            Assert.NotNull(inner2.Obj);
            Assert.NotSame(s1.Obj, inner2.Obj);
        }

        [Fact]
        public void NoCloneStatic()
        {
            var o = SomeStruct.NoCloneObj = new object();
            var s = new SomeStruct(5, new object());
            var c = Clone.Deep(s);
            Assert.Equal(5, c.Int);
            Assert.NotNull(c.Obj);
            Assert.NotSame(s.Obj, c.Obj);
            Assert.Same(o, SomeStruct.NoCloneObj);
        }

        [Fact]
        public void NullableCopyOk()
        {
            var s = new SomeCopyStruct(new object());
            var c = Clone.Deep((SomeCopyStruct?)s);
            Assert.True(c.HasValue);
            Assert.Same(s.Obj, c.Value.Obj);

            var s2 = new {s = (SomeCopyStruct?) s};
            var c2 = Clone.Deep(s2);
            Assert.True(c2.s.HasValue);
            Assert.Same(s.Obj, c2.s.Value.Obj);
        }

        private class NormalClass
        {
            private int _int;
            private readonly int _readonlyInt;

            public NormalClass Test { get; set; }
            public NormalClass ReadonlyTest { get; }

            public CopyClass Copy { get; }
            [CloneType(CloneType.Copy)]
            public NormalClass CopyProp { get; }

            public IgnoreClass Ignore { get; }
            [CloneType(CloneType.Ignore)]
            public NormalClass IgnoreProp { get; }
            [CloneType(CloneType.Ignore)]
            public CopyClass IgnoreCopy { get; }

            [SuppressMessage("ReSharper", "UnusedMember.Local")]
            public int Prop
            {
                get
                {
                    Assert.Equal(_int, _readonlyInt);
                    return _readonlyInt;
                }
                set
                {
                    _int = value;
                    Assert.True(false, "Shouldn't be here");
                }
            }

            public NormalClass(int i, NormalClass test, CopyClass copy, IgnoreClass ignore)
            {
                _int = i;
                _readonlyInt = i;

                Test = test;
                ReadonlyTest = test;

                Copy = copy;
                CopyProp = test;

                Ignore = ignore;
                IgnoreProp = test;
                IgnoreCopy = copy;
            }
        }

        [CloneType(CloneType.Copy)]
        private class CopyClass
        {

        }

        [CloneType(CloneType.Ignore)]
        private class IgnoreClass
        {

        }

        private class WeirdEqual : IEquatable<WeirdEqual>
        {
            public readonly WeirdEqual Parent;
            
            public bool Equals(WeirdEqual other)
            {
                return true;
            }

            public override bool Equals(object obj)
            {
                return true;
            }

            public override int GetHashCode()
            {
                return 0;
            }

            public WeirdEqual(WeirdEqual parent)
            {
                Parent = parent;
            }
        }

        private struct SomeStruct
        {
            public const int NoCloneConst = 5;
            public static object NoCloneObj;

            public readonly int? Int;
            public readonly object Obj;

            public SomeStruct(int? i, object obj)
            {
                Int = i;
                Obj = obj;
            }
        }
        
        [CloneType(CloneType.Copy)]
        private struct SomeCopyStruct
        {
            public readonly object Obj;

            public SomeCopyStruct(object obj)
            {
                Obj = obj;
            }
        }
    }
}