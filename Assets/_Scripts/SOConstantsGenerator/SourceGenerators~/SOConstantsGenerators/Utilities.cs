using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace SOConstantsGenerators;

public static class Utilities
{
    public const string PACKAGE_NAME = "SOConstantsGenerator";

    private static readonly HashSet<string> genertateUITypeAttributeIdentifiers = new()
    {
        "GenerateConstantsFor",
        $"{PACKAGE_NAME}.GenerateConstantsFor",
        "GenerateConstantsForAttribute",
        $"{PACKAGE_NAME}.GenerateConstantsForAttribute",
    };

    public static bool IsTargetNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is ClassDeclarationSyntax classDeclaration
            && IsPartialClass(classDeclaration)
            && HasGenerateConstantsForAttribute(classDeclaration);
    }

    private static bool HasGenerateConstantsForAttribute(ClassDeclarationSyntax classDeclaration)
    {
        int attributeListCount = classDeclaration.AttributeLists.Count;

        for (int i = 0; i < attributeListCount; i++)
        {
            var attributes = classDeclaration.AttributeLists[i].Attributes;
            int attributeCount = attributes.Count;

            for (int j = 0; j < attributeCount; j++)
            {
                var attribute = attributes[j];
                string attributeName = attribute.Name.ToString();

                if (!genertateUITypeAttributeIdentifiers.Contains(attributeName)) continue;

                return true;
            }
        }

        return false;
    }

    public static SOInfo GetSOInfo(GeneratorSyntaxContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;

        return new()
        {
            Name = classDeclaration.Identifier.ToString(),
            Namespace = GetNamespace(classDeclaration),
        };
    }

    public static bool IsPartialClass(ClassDeclarationSyntax classDeclaration)
    {
        return classDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
    }

    public static string GetNamespace(SyntaxNode syntaxNode)
    {
        SyntaxNode parent = syntaxNode.Parent;

        while (parent != null)
        {
            switch (parent)
            {
                case NamespaceDeclarationSyntax namespaceDecl:
                    return namespaceDecl.Name.ToString();
                case FileScopedNamespaceDeclarationSyntax fileScopedNamespaceDecl:
                    return fileScopedNamespaceDecl.Name.ToString();
            }

            parent = parent.Parent;
        }

        return null;
    }

}
