using System.IO;
using System.Linq;
using System.Reflection;
using Unity.Collections.LowLevel.Unsafe;
using UnityEditor;
using UnityEngine;

namespace SOConstantsGenerator;

public static class ConstantsGeneratorHelper
{
    public static void GenerateFile(string outputPath, Object so, string className, string classNamespace)
    {
        var type = so.GetType();
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                         .Where(f => f.GetCustomAttribute<ConstantFieldAttribute>() != null);

        using var writer = new StreamWriter(outputPath, false);

        writer.WriteLine("// This file is auto-generated, do not change.");
        writer.WriteLine($"using System.Runtime.CompilerServices;");
        writer.WriteLine();
        writer.WriteLine($"namespace {classNamespace};");
        writer.WriteLine();

        writer.WriteLine("public static class " + className);
        writer.WriteLine("{");

        foreach (var field in fields)
        {
            var fieldType = field.FieldType;
            var value = field.GetValue(so);

            if (CanBeConst(fieldType))
            {
                // Write const
                writer.WriteLine($"\tpublic const {fieldType} {field.Name} = {FormatValue(value)};");
            }
            else
            {
                // Write static readonly
                byte[] data = ExtractStructBytes(fieldType, value);
                string bytesString = string.Join(", ", data.Select(b => b.ToString()));

                writer.WriteLine($"\tpublic static readonly {fieldType} {field.Name} =");
                writer.WriteLine($"\t\tUnsafe.As<byte, {fieldType}>(ref new byte[] {{ {bytesString} }}[0]);");
            }
        }

        writer.WriteLine("}");
        writer.Flush();
    }

    private static bool CanBeConst(System.Type t)
    {
        return t == typeof(int)
            || t == typeof(float)
            || t == typeof(double)
            || t == typeof(bool)
            || t == typeof(string)
            || t == typeof(char)
            || t == typeof(byte)
            || t == typeof(sbyte)
            || t == typeof(short)
            || t == typeof(ushort)
            || t == typeof(uint)
            || t == typeof(long)
            || t == typeof(ulong);
    }

    private static string FormatStructInitializer(object value, System.Type type)
    {
        var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);

        // Example result:
        // new MyStruct { A = 1, B = 2f, C = "Hello" }
        var assignments = string.Join(", ",
            fields.Select(f => $"{f.Name} = {FormatValue(f.GetValue(value))}"));

        return $"new {type.Name} {{ {assignments} }}";
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            string s => $"\"{s}\"",
            float f => f.ToString("0.######") + "f",
            double d => d.ToString("0.######"),
            _ => value.ToString()
        };
    }

    public static byte[] ExtractStructBytes(System.Type type, object value)
    {
        var method = typeof(ConstantsGeneratorHelper).GetMethod(nameof(StructToBytes), BindingFlags.Static | BindingFlags.Public);
        var generic = method.MakeGenericMethod(type);
        return (byte[])generic.Invoke(null, new object[] { value });
    }

    public static byte[] StructToBytes<T>(T value) where T : struct
    {
        int size = UnsafeUtility.SizeOf<T>();
        byte[] bytes = new byte[size];

        unsafe
        {
            fixed (byte* destPtr = bytes)
            {
                UnsafeUtility.MemCpy(destPtr, UnsafeUtility.AddressOf(ref value), size);
            }
        }

        return bytes;
    }
}