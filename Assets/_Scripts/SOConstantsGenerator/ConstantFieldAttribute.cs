using System;

namespace SOConstantsGenerator;

[AttributeUsage(AttributeTargets.Field)]
public sealed class ConstantFieldAttribute : Attribute
{
}