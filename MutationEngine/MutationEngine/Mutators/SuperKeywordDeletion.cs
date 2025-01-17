using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class SuperKeywordDeletion : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();

        var rewriter = new BaseKeywordRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "ISD";
}

public class BaseKeywordRewriter : CSharpSyntaxRewriter
{
    private ClassDeclarationSyntax _childClass;

    public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
    {
        _childClass = node;
        return base.VisitClassDeclaration(node);
    }

    public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
    {
        if (node.Expression is BaseExpressionSyntax)
        {
            var propertyName = node.Name.Identifier.Text;
            var propertyExistsInChild = _childClass.Members
                .OfType<PropertyDeclarationSyntax>()
                .Any(p => p.Identifier.Text == propertyName);

            if (propertyExistsInChild)
            {
                return node.Name;
            }
        }

        return base.VisitMemberAccessExpression(node);
    }
}