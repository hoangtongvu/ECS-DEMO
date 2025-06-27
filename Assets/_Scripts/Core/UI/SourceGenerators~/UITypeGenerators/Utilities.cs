using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace UITypeGenerators
{
    public static class Utilities
    {
        private static readonly HashSet<string> genertateUITypeAttributeIdentifiers = new()
        {
            "GenerateUIType",
            "Core.UI.GenerateUIType",
            "GenerateUITypeAttribute",
            "Core.UI.GenerateUITypeAttribute",
        };

        public static bool IsTargetNode(SyntaxNode syntaxNode)
        {
            return syntaxNode is ClassDeclarationSyntax classDeclaration
                && IsPartialClass(classDeclaration)
                && HasGenertateUITypeAttribute(classDeclaration);
        }

        private static bool HasGenertateUITypeAttribute(ClassDeclarationSyntax classDeclaration)
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
                    if (attribute.ArgumentList?.Arguments.Count != 1) continue;

                    return true;
                }

            }

            return false;
        }

        public static ConcreteUIInfo GetConcreteUIInfo(GeneratorSyntaxContext context)
        {
            var classDeclaration = (ClassDeclarationSyntax)context.Node;

            return new()
            {
                ConcreteUICtrlName = classDeclaration.Identifier.ToString(),
                ConcreteUICtrlNamespace = GetNamespace(classDeclaration),
                UITypeName = GetUITypeName(classDeclaration),
            };

        }

        private static string GetUITypeName(ClassDeclarationSyntax classDeclaration)
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

                    if (!genertateUITypeAttributeIdentifiers.Contains(attributeName)
                        && attribute.ArgumentList?.Arguments.Count == 1) continue;

                    if (attribute.ArgumentList.Arguments[0].Expression is not LiteralExpressionSyntax literalExpression)
                        return null;

                    if (!literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
                        return null;

                    return literalExpression.Token.ValueText;

                }

            }

            return null;

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
