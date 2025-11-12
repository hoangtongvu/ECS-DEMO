using System;

namespace SOConstGenerator;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ConstantFieldAttribute : Attribute
{
}