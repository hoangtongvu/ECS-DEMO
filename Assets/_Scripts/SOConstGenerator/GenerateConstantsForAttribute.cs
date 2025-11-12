using System;

namespace SOConstGenerator;

[AttributeUsage(AttributeTargets.Class)]
public sealed class GenerateConstantsForAttribute : Attribute
{
    public string ConstHolderClassName;
    public string ConstHolderClassNamespace;

    public GenerateConstantsForAttribute(string className, string classNamespace)
    {
        ConstHolderClassName = className;
        ConstHolderClassNamespace = classNamespace;
    }
}