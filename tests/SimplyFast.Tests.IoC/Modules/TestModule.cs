﻿using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using SF.IoC;
using SF.IoC.Modules;

namespace SF.Tests.IoC.Modules
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class TestModule: FastModule
    {
        public class TestEnumerable : IEnumerable<string>
        {
            public IEnumerator<string> GetEnumerator()
            {
                yield return "str1";
                yield return "str2";
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        public override void Load()
        {
            Bind<string>().ToConstant("test");
            Bind<IEnumerable<string>>().To<TestEnumerable>();
        }
    }
}