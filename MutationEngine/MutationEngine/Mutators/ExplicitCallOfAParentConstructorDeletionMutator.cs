using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class ExplicitCallOfAParentConstructorDeletionMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot();

        var rewriter = new ConstructorBaseCallRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }

    private class ConstructorBaseCallRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var initializer = node.Initializer;
            if (initializer != null && initializer.ThisOrBaseKeyword.IsKind(SyntaxKind.BaseKeyword))
            {
                return node.WithInitializer(null);
            }

            return base.VisitConstructorDeclaration(node);
        }
    }

    public string Name { get; init; } = "IPC";
}