using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace UITypeGenerator
{
    public class GenerateUITypeAttributeSyntaxReceiver : ISyntaxReceiver
    {
        public List<ClassDeclarationSyntax> ClassSyntaxes { get; } = new List<ClassDeclarationSyntax>();
        public List<string> UITypeNames { get; } = new List<string>();

        public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
        {
            if (syntaxNode is ClassDeclarationSyntax classDeclaration)
            {
                if (!IsPartialClass(classDeclaration)) return;
                if (!this.HasGenertateUITypeAttribute(classDeclaration, out string uiTypeName)) return;
                this.ClassSyntaxes.Add(classDeclaration);
                this.UITypeNames.Add(uiTypeName);
            }
                
        }

        private bool IsPartialClass(ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PartialKeyword));
        }

        private bool HasGenertateUITypeAttribute(ClassDeclarationSyntax classDeclaration, out string uiTypeName)
        {
            uiTypeName = null;
            string tempString = null;

            if (!classDeclaration.AttributeLists.Any(attributeList =>
            {
                return attributeList.Attributes.Any(attribute =>
                {
                    if (attribute.Name.ToString() == "GenerateUIType" &&
                        attribute.ArgumentList?.Arguments.Count == 1 &&
                        attribute.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literalExpression &&
                        literalExpression.IsKind(SyntaxKind.StringLiteralExpression))
                    {
                        tempString = literalExpression.Token.ValueText;
                        return true;
                    };

                    return false;
                });
            })) return false;

            uiTypeName = tempString;
            return true;

        }

    }

}