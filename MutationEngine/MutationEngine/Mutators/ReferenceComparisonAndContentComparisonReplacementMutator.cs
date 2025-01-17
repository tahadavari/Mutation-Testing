using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class ReferenceComparisonAndContentComparisonReplacementMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    public string Name { get; init; } = "EOC";

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        if (root == null)
            throw new InvalidOperationException("Failed to parse the code.");

        var rewriter = new ReferenceComparisonAndContentComparisonReplacementRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }
}

public class ReferenceComparisonAndContentComparisonReplacementRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitBinaryExpression(BinaryExpressionSyntax node)
    {
        if (node.IsKind(SyntaxKind.EqualsExpression))
        {
            var left = node.Left;
            var right = node.Right;

            var equalsInvocation = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    left,
                    SyntaxFactory.IdentifierName("Equals")),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SingletonSeparatedList(
                        SyntaxFactory.Argument(right))));

            return equalsInvocation.WithTriviaFrom(node);
        }

        return base.VisitBinaryExpression(node);
    }
}