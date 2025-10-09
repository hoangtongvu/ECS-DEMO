using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DInteractionGenerators;

public static class Utilities
{
    public const string PACKAGE_NAME = "DInteraction";

    private static readonly HashSet<string> genertateUITypeAttributeIdentifiers = new()
    {
        "InteractionPhase",
        $"{PACKAGE_NAME}.InteractionPhase",
        "InteractionPhaseAttribute",
        $"{PACKAGE_NAME}.InteractionPhaseAttribute",
    };

    public static bool IsTargetNode(SyntaxNode syntaxNode)
    {
        return syntaxNode is StructDeclarationSyntax structDeclaration
            && IsPartialStruct(structDeclaration)
            && HasInteractionPhaseAttribute(structDeclaration);
    }

    private static bool HasInteractionPhaseAttribute(StructDeclarationSyntax structDeclaration)
    {
        int attributeListCount = structDeclaration.AttributeLists.Count;

        for (int i = 0; i < attributeListCount; i++)
        {
            var attributes = structDeclaration.AttributeLists[i].Attributes;
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

    public static InteractionPhaseInfo GetInteractionPhaseInfo(GeneratorSyntaxContext context)
    {
        var structDeclaration = (StructDeclarationSyntax)context.Node;

        return new()
        {
            Name = structDeclaration.Identifier.ToString(),
            Namespace = GetNamespace(structDeclaration),
        };
    }

    public static bool IsPartialStruct(StructDeclarationSyntax structDeclaration)
    {
        return structDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
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
