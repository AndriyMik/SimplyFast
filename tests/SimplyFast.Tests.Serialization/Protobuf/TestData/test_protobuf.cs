﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using ProtoBuf;

// Generated from: test.proto
namespace SF.Tests.Serialization.Protobuf.TestData
{
    [Serializable, ProtoContract(Name = @"TestMessage", UseProtoMembersOnly = true)]
    public partial class PTestMessage
    {

        private double _fdouble;
        [ProtoMember(1, IsRequired = true, Name = @"fdouble", DataFormat = DataFormat.TwosComplement)]
        public double Fdouble
        {
            get { return _fdouble; }
            set { _fdouble = value; }
        }

        private float _ffloat;
        [ProtoMember(2, IsRequired = true, Name = @"ffloat", DataFormat = DataFormat.FixedSize)]
        public float Ffloat
        {
            get { return _ffloat; }
            set { _ffloat = value; }
        }

        private int _fint32;
        [ProtoMember(3, IsRequired = true, Name = @"fint32", DataFormat = DataFormat.TwosComplement)]
        public int Fint32
        {
            get { return _fint32; }
            set { _fint32 = value; }
        }

        private long? _fint64;
        [ProtoMember(4, IsRequired = false, Name = @"fint64", DataFormat = DataFormat.TwosComplement)]
        public long? Fint64
        {
            get { return _fint64; }
            set { _fint64 = value; }
        }

        private uint? _fuint32;
        [ProtoMember(5, IsRequired = false, Name = @"fuint32", DataFormat = DataFormat.TwosComplement)]
        public uint? Fuint32
        {
            get { return _fuint32; }
            set { _fuint32 = value; }
        }

        private ulong? _fuint64;
        [ProtoMember(6, IsRequired = false, Name = @"fuint64", DataFormat = DataFormat.TwosComplement)]
        public ulong? Fuint64
        {
            get { return _fuint64; }
            set { _fuint64 = value; }
        }

        private int? _fsint32;
        [ProtoMember(7, IsRequired = false, Name = @"fsint32", DataFormat = DataFormat.ZigZag)]
        public int? Fsint32
        {
            get { return _fsint32; }
            set { _fsint32 = value; }
        }

        private long? _fsint64;
        [ProtoMember(8, IsRequired = false, Name = @"fsint64", DataFormat = DataFormat.ZigZag)]
        public long? Fsint64
        {
            get { return _fsint64; }
            set { _fsint64 = value; }
        }

        private uint? _ffixed32;
        [ProtoMember(9, IsRequired = false, Name = @"ffixed32", DataFormat = DataFormat.FixedSize)]
        public uint? Ffixed32
        {
            get { return _ffixed32; }
            set { _ffixed32 = value; }
        }

        private ulong? _ffixed64;
        [ProtoMember(10, IsRequired = false, Name = @"ffixed64", DataFormat = DataFormat.FixedSize)]
        public ulong? Ffixed64
        {
            get { return _ffixed64; }
            set { _ffixed64 = value; }
        }

        private int? _fsfixed32;
        [ProtoMember(11, IsRequired = false, Name = @"fsfixed32", DataFormat = DataFormat.FixedSize)]
        public int? Fsfixed32
        {
            get { return _fsfixed32; }
            set { _fsfixed32 = value; }
        }

        private long? _fsfixed64;
        [ProtoMember(12, IsRequired = false, Name = @"fsfixed64", DataFormat = DataFormat.FixedSize)]
        public long? Fsfixed64
        {
            get { return _fsfixed64; }
            set { _fsfixed64 = value; }
        }

        private bool? _fbool;
        [ProtoMember(13, IsRequired = false, Name = @"fbool", DataFormat = DataFormat.Default)]
        public bool? Fbool
        {
            get { return _fbool; }
            set { _fbool = value; }
        }

        private string _fstring;
        [ProtoMember(14, IsRequired = false, Name = @"fstring", DataFormat = DataFormat.Default)]
        public string Fstring
        {
            get { return _fstring; }
            set { _fstring = value; }
        }

        private byte[] _fbytes;
        [ProtoMember(15, IsRequired = false, Name = @"fbytes", DataFormat = DataFormat.Default)]
        public byte[] Fbytes
        {
            get { return _fbytes; }
            set { _fbytes = value; }
        }

        private TestEnum? _fenum;
        [ProtoMember(16, IsRequired = false, Name = @"fenum", DataFormat = DataFormat.TwosComplement)]
        public TestEnum? Fenum
        {
            get { return _fenum; }
            set { _fenum = value; }
        }

        private InnerMessage _finner;
        [ProtoMember(17, IsRequired = false, Name = @"finner", DataFormat = DataFormat.Default)]
        public InnerMessage Finner
        {
            get { return _finner; }
            set { _finner = value; }
        }

        private List<InnerMessage> _frep;
        [ProtoMember(18, Name = @"frep", DataFormat = DataFormat.Default)]
        public List<InnerMessage> Frep
        {
            get { return _frep; }
            set { _frep = value; }
        }


        private List<TestEnum> _frepEnum;
        [ProtoMember(19, Name = @"frep_enum", DataFormat = DataFormat.TwosComplement)]
        public List<TestEnum> FrepEnum
        {
            get { return _frepEnum; }
            set { _frepEnum = value; }
        }


        private List<string> _frepString;
        [ProtoMember(20, Name = @"frep_string", DataFormat = DataFormat.Default)]
        public List<string> FrepString
        {
            get { return _frepString; }
            set { _frepString = value; }
        }


        private List<uint> _frepFixed32;
        [ProtoMember(21, Name = @"frep_fixed32", DataFormat = DataFormat.FixedSize)]
        public List<uint> FrepFixed32
        {
            get { return _frepFixed32; }
            set { _frepFixed32 = value; }
        }


        private List<uint> _frepUint32;
        [ProtoMember(22, Name = @"frep_uint32", DataFormat = DataFormat.TwosComplement)]
        public List<uint> FrepUint32
        {
            get { return _frepUint32; }
            set { _frepUint32 = value; }
        }

    }

}