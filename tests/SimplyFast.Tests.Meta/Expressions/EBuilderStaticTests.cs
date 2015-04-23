﻿using System;
using System.Globalization;
using NUnit.Framework;
using SF.Expressions;
using SF.Expressions.Dynamic;

namespace SF.Tests.Expressions
{
    [TestFixture]
    public class EBuilderStaticTests
    {
        public class TestClass
        {
            public TestClass(int value)
            {
                TestField = value;
            }

            public static int TestField;
            public static string TestProp
            {
                get { return TestField.ToString(CultureInfo.InvariantCulture); }
                set { TestField = int.Parse(value); }
            }

            public static bool IsOk()
            {
                return true;
            }

            public static double TestMethod(float param)
            {
                return TestField * param;
            }

            public static Action<bool> TestAction;
        }

        [Test]
        public void TestGetSetField()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestField = testClass.TestField;
            });
            Assert.AreEqual("() => (TestClass.TestField = TestClass.TestField)", lambda.ToDebugString());
        }

        [Test]
        public void TestGetSetProperty()
        {
            var lambda = EBuilder.Lambda(() =>
                {
                    var testClass = typeof(TestClass).EBuilder();
                    return testClass.TestProp = testClass.TestProp;
            });
            Assert.AreEqual("() => (TestClass.TestProp = TestClass.TestProp)", lambda.ToDebugString());
        }

        [Test]
        public void TestMethod()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestMethod(2.0f);
            });
            Assert.AreEqual("() => TestClass.TestMethod(2F)", lambda.ToDebugString());
        }

        [Test]
        public void TestAction()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass.TestAction(true);
            });
            Assert.AreEqual("() => TestClass.TestAction(True)", lambda.ToDebugString());
        }

        [Test]
        public void TestNew()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(TestClass).EBuilder();
                return testClass(2);
            });
            Assert.AreEqual("() => new TestClass(2)", lambda.ToDebugString());
        }

        [Test]
        public void TestNewArray()
        {
            var lambda = EBuilder.Lambda(() =>
            {
                var testClass = typeof(byte[]).EBuilder();
                return testClass(2);
            });
            Assert.AreEqual("() => new Byte[2]", lambda.ToDebugString());
        }
    }
}