using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class SuperKeywordInsertionMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        Console.WriteLine(Mutate(originalContent));
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("MyCompilation", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        if (root == null)
            throw new InvalidOperationException("Failed to parse the code.");

        var rewriter = new SuperKeywordInsertionRewriter(semanticModel);
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "ISI";
}
public class SuperKeywordInsertionRewriter : CSharpSyntaxRewriter
{
    private SemanticModel SemanticModel { get; }

    public SuperKeywordInsertionRewriter(SemanticModel semanticModel)
    {
        SemanticModel = semanticModel;
    }

    public override SyntaxNode VisitIdentifierName(IdentifierNameSyntax node)
    {
        var symbolInfo = SemanticModel.GetSymbolInfo(node);
        var symbol = symbolInfo.Symbol;

        if (symbol != null && symbol.Kind == SymbolKind.Field)
        {
            var containingType = symbol.ContainingType;
            var baseType = containingType.BaseType;

            if (baseType != null && baseType.GetMembers().Any(m => m.Name == symbol.Name))
            {
                var superMemberAccess = SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("super"),
                    node);

                return superMemberAccess.WithTriviaFrom(node);
            }
        }

        return base.VisitIdentifierName(node);
    }
}