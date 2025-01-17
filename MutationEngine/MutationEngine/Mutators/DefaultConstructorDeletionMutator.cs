using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class DefaultConstructorDeletionMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        if (root == null)
            throw new InvalidOperationException("Failed to parse the code.");

        var rewriter = new DefaultConstructorRemovalRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "JDC";
}

public class DefaultConstructorRemovalRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
    {
        // Check if the constructor is a default constructor (no parameters)
        if (!node.ParameterList.Parameters.Any())
        {
            return null; // Remove the default constructor
        }

        return base.VisitConstructorDeclaration(node);
    }
}