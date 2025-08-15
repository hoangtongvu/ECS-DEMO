using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace DReactionGenerators
{
    public static class Utilities
    {
        private static readonly HashSet<string> genertateUITypeAttributeIdentifiers = new()
        {
            "ReactionComponents",
            "DReaction.ReactionComponents",
            "ReactionComponentsAttribute",
            "DReaction.ReactionComponentsAttribute",
        };

        public static bool IsTargetNode(SyntaxNode syntaxNode)
        {
            return syntaxNode is StructDeclarationSyntax structDeclaration
                && IsPartialStruct(structDeclaration)
                && HasReactionComponentsAttribute(structDeclaration);
        }

        private static bool HasReactionComponentsAttribute(StructDeclarationSyntax structDeclaration)
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

        public static ReactionComponentsContainerInfo GetReactionComponentsContainerInfo(GeneratorSyntaxContext context)
        {
            var structDeclaration = (StructDeclarationSyntax)context.Node;

            return new()
            {
                ContainerName = structDeclaration.Identifier.ToString(),
                ContainerNamespace = GetNamespace(structDeclaration),
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
                if (parent is NamespaceDeclarationSyntax namespaceDeclaration)
                    return namespaceDeclaration.Name.ToString();

                parent = parent.Parent;

            }

            return null;

        }

        public static void GetNameAndNamespaceOfGenericArgument(
            SemanticModel semanticModel
            , ExpressionSyntax expressionSyntax
            , out string typeName
            , out string namespaceName)
        {
            ITypeSymbol typeSymbol = semanticModel.GetTypeInfo(expressionSyntax).Type;
            if (typeSymbol != null)
            {
                typeName = typeSymbol.Name;
                namespaceName = typeSymbol.ContainingNamespace?.ToDisplayString() ?? "(NoNamespace)";
                return;
            }

            throw new System.Exception($"Can not resolve {nameof(ITypeSymbol)} for Generic argument");

        }

    }

}
