﻿using System;
using System.Diagnostics.CodeAnalysis;
using SimplyFast.IO;

namespace SimplyFast.Tests.IO
{
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class FastBufferReaderTestsSubBuffer: FastBufferReaderTests
    {
        private byte[] _buffer;
        protected override FastBufferReader Buf(params byte[] bytes)
        {
            _buffer = new byte[bytes.Length + 10];
            Array.Copy(bytes, 0, _buffer, 5, bytes.Length);
            return new FastBufferReader(_buffer, 5, bytes.Length);
        }
    }
}