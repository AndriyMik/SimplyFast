﻿using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SF.Reflection.Emit
{
    /// <summary>
    ///     Helper with usefull Emit methods
    /// </summary>
    public static class ILGeneratorEx
    {
        public static void EmitMethodCall(this ILGenerator generator, MethodInfo methodInfo)
        {
            var code = methodInfo.IsVirtual ? OpCodes.Callvirt : OpCodes.Call;
            generator.Emit(code, methodInfo);
        }

        public static void EmitFieldGet(this ILGenerator generator, FieldInfo fieldInfo)
        {
            var code = fieldInfo.IsStatic ? OpCodes.Ldsfld : OpCodes.Ldfld;
            generator.Emit(code, fieldInfo);
        }

        public static void EmitFieldSet(this ILGenerator generator, FieldInfo fieldInfo)
        {
            var code = fieldInfo.IsStatic ? OpCodes.Stsfld : OpCodes.Stfld;
            generator.Emit(code, fieldInfo);
        }

        public static void EmitBox(this ILGenerator generator, Type type)
        {
            if (type.IsValueType)
                generator.Emit(OpCodes.Box, type);
        }

        public static void EmitUnBoxAnyOrCastClass(this ILGenerator generator, Type type)
        {
            var code = type.IsValueType ? OpCodes.Unbox_Any : OpCodes.Castclass;
            generator.Emit(code, type);
        }

        public static void EmitLdConst(this ILGenerator generator, object value)
        {
            var type = value.GetType();
            try
            {
                if (type.IsEnum)
                    type = Enum.GetUnderlyingType(type);
                if (type == typeof (string))
                    generator.Emit(OpCodes.Ldstr, (string) value);
                else if (type == typeof (long))
                    generator.Emit(OpCodes.Ldc_I8, (long) value);
                else if (type == typeof (ulong))
                    generator.Emit(OpCodes.Ldc_I8, unchecked((long) (ulong) value));
                else if (type == typeof (float))
                    generator.Emit(OpCodes.Ldc_R4, (float) value);
                else if (type == typeof (double))
                    generator.Emit(OpCodes.Ldc_R8, (double) value);
                else if (type == typeof (bool))
                    generator.EmitLdcI4((bool) value ? 1 : 0);
                else if (type == typeof (char))
                    generator.EmitLdcI4((char) value);
                else if (type == typeof (byte))
                    generator.EmitLdcI4((byte) value);
                else if (type == typeof (sbyte))
                    generator.EmitLdcI4((sbyte) value);
                else if (type == typeof (short))
                    generator.EmitLdcI4((short) value);
                else if (type == typeof (ushort))
                    generator.EmitLdcI4((ushort) value);
                else if (type == typeof (int))
                    generator.EmitLdcI4((int) value);
                else if (type == typeof (uint))
                    generator.EmitLdcI4(unchecked((int) (uint) value));
                else
                    throw new ArgumentException("Type not supported.", "value");
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Invalid value", "value", ex);
            }
        }

        public static void EmitLdcI4(this ILGenerator generator, int value)
        {
            switch (value)
            {
                case -1:
                    generator.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value >= -128 && value <= 127)
                        generator.Emit(OpCodes.Ldc_I4_S, (byte) value);
                    else
                        generator.Emit(OpCodes.Ldc_I4, value);
                    break;
            }
        }

        public static void EmitStloc(this ILGenerator generator, int index)
        {
            if (index < 0)
                throw new ArgumentException("index");
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (index <= 255)
                        generator.Emit(OpCodes.Stloc_S, index);
                    else if (index <= 65534) // not 65535 - read MSDN
                        generator.Emit(OpCodes.Stloc, index);
                    else
                        throw new ArgumentException("index");
                    break;
            }
        }

        public static void EmitLdloc(this ILGenerator generator, int index)
        {
            if (index < 0)
                throw new ArgumentException("index");
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index <= 255)
                        generator.Emit(OpCodes.Ldloc_S, index);
                    else if (index <= 65534) // not 65535 - read MSDN
                        generator.Emit(OpCodes.Ldloc, index);
                    else
                        throw new ArgumentException("index");
                    break;
            }
        }

        public static void EmitLdarg(this ILGenerator generator, int index)
        {
            if (index < 0)
                throw new ArgumentException("index");
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= 255)
                        generator.Emit(OpCodes.Ldarg_S, index);
                    else if (index <= 65535)
                        generator.Emit(OpCodes.Ldarg, index);
                    else
                        throw new ArgumentException("index");
                    break;
            }
        }

        public static void EmitLdloca(this ILGenerator generator, int index)
        {
            if (index < 0)
                throw new ArgumentException("index");
            if (index <= 255)
                generator.Emit(OpCodes.Ldloca_S, index);
            else if (index <= 65534) // not 65535 - read MSDN
                generator.Emit(OpCodes.Ldloca, index);
            else
                throw new ArgumentException("index");
        }

        public static void EmitLdind(this ILGenerator generator, Type type)
        {
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            if (!type.IsValueType)
                generator.Emit(OpCodes.Ldind_Ref);
            else if (type == typeof (long))
                generator.Emit(OpCodes.Ldind_I8);
            else if (type == typeof (ulong))
                generator.Emit(OpCodes.Ldind_I8);
            else if (type == typeof (float))
                generator.Emit(OpCodes.Ldind_R4);
            else if (type == typeof (double))
                generator.Emit(OpCodes.Ldind_R8);
            else if (type == typeof (bool))
                generator.Emit(OpCodes.Ldind_I1);
            else if (type == typeof (char))
                generator.Emit(OpCodes.Ldind_U2);
            else if (type == typeof (byte))
                generator.Emit(OpCodes.Ldind_U1);
            else if (type == typeof (sbyte))
                generator.Emit(OpCodes.Ldind_I1);
            else if (type == typeof (short))
                generator.Emit(OpCodes.Ldind_I2);
            else if (type == typeof (ushort))
                generator.Emit(OpCodes.Ldind_U2);
            else if (type == typeof (int))
                generator.Emit(OpCodes.Ldind_I4);
            else if (type == typeof (uint))
                generator.Emit(OpCodes.Ldind_U4);
            else if (type.IsValueType)
                generator.Emit(OpCodes.Ldobj, type);
        }

        public static void EmitStind(this ILGenerator generator, Type type)
        {
            if (type.IsEnum)
                type = Enum.GetUnderlyingType(type);
            if (!type.IsValueType)
                generator.Emit(OpCodes.Stind_Ref);
            else if (type == typeof (long))
                generator.Emit(OpCodes.Stind_I8);
            else if (type == typeof (ulong))
                generator.Emit(OpCodes.Stind_I8);
            else if (type == typeof (float))
                generator.Emit(OpCodes.Stind_R4);
            else if (type == typeof (double))
                generator.Emit(OpCodes.Stind_R8);
            else if (type == typeof (bool))
                generator.Emit(OpCodes.Stind_I1);
            else if (type == typeof (char))
                generator.Emit(OpCodes.Stind_I2);
            else if (type == typeof (byte))
                generator.Emit(OpCodes.Stind_I1);
            else if (type == typeof (sbyte))
                generator.Emit(OpCodes.Stind_I1);
            else if (type == typeof (short))
                generator.Emit(OpCodes.Stind_I2);
            else if (type == typeof (ushort))
                generator.Emit(OpCodes.Stind_I2);
            else if (type == typeof (int))
                generator.Emit(OpCodes.Stind_I4);
            else if (type == typeof (uint))
                generator.Emit(OpCodes.Stind_I4);
            else if (type.IsValueType)
                generator.Emit(OpCodes.Ldobj, type);
        }

        public static void EmitLdarga(this ILGenerator generator, int index)
        {
            if (index < 0)
                throw new ArgumentException("index");
            if (index <= 255)
                generator.Emit(OpCodes.Ldarga_S, index);
            else if (index <= 65534) // not 65535 - read MSDN
                generator.Emit(OpCodes.Ldarga, index);
            else
                throw new ArgumentException("index");
        }
    }
}