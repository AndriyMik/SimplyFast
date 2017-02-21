﻿using System;
using System.IO;
using NUnit.Framework;
using SF.Serialization;
using SF.Tests.Serialization.Protobuf.TestData;

namespace SF.Tests.Serialization.Protobuf
{
    [TestFixture]
    public class TestAgainstProtobufNet : MessageTests
    {
        protected override void Test(FTestMessage message, Action<FTestMessage> customAssert = null)
        {
            var d1 = TestFastWriteProtoRead(message);
            AssertDeserialized(message, d1, customAssert);
            var d2 = TestProtoWriteFastRead(message);
            AssertDeserialized(message, d2, customAssert);
        }

        private static FTestMessage TestProtoWriteFastRead(FTestMessage message)
        {
            using (var ms = new MemoryStream())
            {
                var pmsg = message.ToProtoNet();
                ProtoBuf.Serializer.Serialize(ms, pmsg);
                return pmsg.ToMessage();
            }
        }

        private static FTestMessage TestFastWriteProtoRead(FTestMessage message)
        {
            var serialized = ProtoSerializer.Serialize(message);
            using (var ms = new MemoryStream(serialized))
            {
                var deserialized = ProtoBuf.Serializer.Deserialize<PTestMessage>(ms);
                var fmsg = deserialized.ToMessage();
                return fmsg;
            }
        }
    }
}