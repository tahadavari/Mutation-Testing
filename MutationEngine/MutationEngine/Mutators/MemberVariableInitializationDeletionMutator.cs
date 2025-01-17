using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class MemberVariableInitializationDeletionMutator : IMutator
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

        var rewriter = new InitializationRemovalRewriter();
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "JID";
}

public class InitializationRemovalRewriter : CSharpSyntaxRewriter
{
    public override SyntaxNode VisitFieldDeclaration(FieldDeclarationSyntax node)
    {
        var variables = node.Declaration.Variables;
        var newVariables = variables.Select(v => v.WithInitializer(null)).ToList();
        var newDeclaration = node.Declaration.WithVariables(SyntaxFactory.SeparatedList(newVariables));
        return node.WithDeclaration(newDeclaration);
    }

    public override SyntaxNode VisitPropertyDeclaration(PropertyDeclarationSyntax node)
    {
        if (node.Initializer != null)
        {
            return node.WithInitializer(null).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.None));
        }

        return base.VisitPropertyDeclaration(node);
    }
}