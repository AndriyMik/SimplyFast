using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Xunit;
using SimplyFast.Reflection.Tests.TestData;

namespace SimplyFast.Reflection.Tests
{
    
    public class FieldInfoExTests
    {
        [Fact]
        public void ConstGettersWorks()
        {
            var type = typeof(SomeClassWithConsts);
            Func<string, Func<object>> getGetter = x => type.Field(x).GetterAs<Func<object>>();
            Assert.Equal(SomeClassWithConsts.TestBool, getGetter("TestBool")());

            Assert.Equal(SomeClassWithConsts.TestC, getGetter("TestC")());
            Assert.Equal(SomeClassWithConsts.TestStr, getGetter("TestStr")());

            Assert.Equal(SomeClassWithConsts.TestB, getGetter("TestB")());
            Assert.Equal(SomeClassWithConsts.TestSbyte, getGetter("TestSbyte")());
            Assert.Equal(SomeClassWithConsts.TestS, getGetter("TestS")());
            Assert.Equal(SomeClassWithConsts.TestUShort, getGetter("TestUShort")());
            Assert.Equal(SomeClassWithConsts.TestI, getGetter("TestI")());
            Assert.Equal(SomeClassWithConsts.TestUInt, getGetter("TestUInt")());
            Assert.Equal(SomeClassWithConsts.TestL, getGetter("TestL")());
            Assert.Equal(SomeClassWithConsts.TestULong, getGetter("TestULong")());

            Assert.Equal(SomeClassWithConsts.TestF, getGetter("TestF")());
            Assert.Equal(SomeClassWithConsts.TestD, getGetter("TestD")());
            Assert.Equal(SomeClassWithConsts.TestDec, getGetter("TestDec")());
            Assert.Equal(SomeClassWithConsts.TestEnum, getGetter("TestEnum")());
        }

        [Fact]
        public void ConstSettersDoesNotExists()
        {
            var type = typeof(SomeClassWithConsts);
            Func<string, Action<object>> getSetter = x => type.Field(x).SetterAs<Action<object>>();
            Assert.Null(getSetter("TestBool"));

            Assert.Null(getSetter("TestC"));
            Assert.Null(getSetter("TestStr"));

            Assert.Null(getSetter("TestB"));
            Assert.Null(getSetter("TestSbyte"));
            Assert.Null(getSetter("TestS"));
            Assert.Null(getSetter("TestUShort"));
            Assert.Null(getSetter("TestI"));
            Assert.Null(getSetter("TestUInt"));
            Assert.Null(getSetter("TestL"));
            Assert.Null(getSetter("TestULong"));

            Assert.Null(getSetter("TestF"));
            Assert.Null(getSetter("TestD"));
            Assert.Null(getSetter("TestDec"));
            Assert.Null(getSetter("TestEnum"));
        }

        [Fact]
        public void GetterExists()
        {
            Assert.NotNull(typeof(SomeClass1).Field("_f1").GetterAs<Func<object, object>>());
            Assert.NotNull(typeof(SomeClass1).Field("F2").GetterAs<Func<object, object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").GetterAs<Func<object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").GetterAs(typeof(Func<object>)));
        }

        [Fact]
        public void GetterThrowsIfWrongType()
        {
            var d = new SomeClass1();
            Assert.Throws<InvalidCastException>(() => typeof(TestClass2).Field("_f1").GetterAs<Func<object, object>>()(d));
        }

        [Fact]
        public void GetterWorksForPrivate()
        {
            var c = new SomeClass1();
            Assert.Equal(1,
                            typeof(SomeClass1).Field("_f1").GetterAs<Func<SomeClass1, object>>()(c));
        }

        [Fact]
        public void GetterWorksForPrivateStatic()
        {
            Assert.Equal("_f3t", typeof(TestClass2).Field("_f3").GetterAs<Func<object>>()());
        }

        [Fact]
        public void GetterWorksForPublic()
        {
            var c = new SomeClass1();
            Assert.Equal("test", typeof(SomeClass1).Field("F2").GetterAs<Func<SomeClass1, string>>()(c));
        }

        [Fact]
        public void SetterExists()
        {
            Assert.NotNull(typeof(SomeClass1).Field("_f1").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(SomeClass1).Field("F2").SetterAs<Action<object, object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").SetterAs<Action<object>>());
            Assert.NotNull(typeof(TestClass2).Field("_f3").SetterAs(typeof(Action<object>)));
        }

        [Fact]
        public void SetterThrowsIfWrongType()
        {
            var c = new SomeClass1();
            var d = new TestClass2();
            Assert.Throws<InvalidCastException>(
                () => typeof(TestClass2).Field("_f1").SetterAs<Action<object, object>>()(c, 123));
            Assert.Throws<InvalidCastException>(
                () => typeof(TestClass2).Field("_f1").SetterAs<Action<object, object>>()(d, "te"));
        }

        [Fact]
        public void SetterWorksForPrivate()
        {
            var c = new SomeClass1();
            typeof(SomeClass1).Field("_f1").SetterAs<Action<SomeClass1, int>>()(c, 2);
            Assert.Equal(2, typeof(SomeClass1).Field("_f1").GetterAs<Func<SomeClass1, int>>()(c));
        }

        [Fact]
        public void SetterWorksForPrivateStatic()
        {
            try
            {
                typeof(TestClass2).Field("_f3").SetterAs<Action<int>>()(123);
                Assert.Equal(123, typeof(TestClass2).Field("_f3").GetterAs<Func<object>>()());
            }
            finally
            {
                TestClass2.F3 = "_f3t";
            }
        }

        [Fact]
        public void SetterWorksForPublic()
        {
            var c = new SomeClass1();
            typeof(SomeClass1).Field("F2").SetterAs<Action<SomeClass1, object>>()(c, "te");
            Assert.Equal("te", typeof(SomeClass1).Field("F2").GetterAs<Func<object, string>>()(c));
        }

        [Fact]
        public void FieldsCacheFine()
        {
            var fields = new HashSet<FieldInfo>(typeof(SomeClass1).Fields());
            Assert.True(fields.SetEquals(typeof(SomeClass1).GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)));
        }

        private class ClassWithFields
        {
#pragma warning disable 169
#pragma warning disable 414
            public int Ok = 1;
            public readonly int Read = 2;
            [SuppressMessage("ReSharper", "UnusedMember.Local")]
            public const int Constant = 3;
#pragma warning restore 169
#pragma warning restore 414
        }

        [Fact]
        public void CanWriteTests()
        {
            var type = typeof(ClassWithFields);
            Assert.True(type.Field("Ok").CanWrite());
            Assert.False(type.Field("Read").CanWrite());
            Assert.False(type.Field("Constant").CanWrite());
        }
    }
}