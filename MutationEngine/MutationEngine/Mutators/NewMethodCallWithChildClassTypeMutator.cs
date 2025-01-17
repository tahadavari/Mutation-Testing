using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class NewMethodCallWithChildClassTypeMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        Console.WriteLine(Mutate(originalContent));
        return Mutate(originalContent);
    }

    public string Name { get; init; } = "PNC";

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var compilation = CSharpCompilation.Create("MyCompilation", new[] { syntaxTree });
        var semanticModel = compilation.GetSemanticModel(syntaxTree);

        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        if (root == null)
            throw new InvalidOperationException("Failed to parse the code.");

        var rewriter = new NewMethodCallWithChildClassTypeRewriter(semanticModel);
        var newRoot = rewriter.Visit(root);

        return newRoot.NormalizeWhitespace().ToFullString();
    }
}
public class NewMethodCallWithChildClassTypeRewriter : CSharpSyntaxRewriter
{
    private SemanticModel SemanticModel { get; }

    public NewMethodCallWithChildClassTypeRewriter(SemanticModel semanticModel)
    {
        SemanticModel = semanticModel;
    }

    public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
    {
        var symbolInfo = SemanticModel.GetSymbolInfo(node);
        var symbol = symbolInfo.Symbol as IMethodSymbol;

        if (symbol != null)
        {
            var containingType = symbol.ContainingType;
            var childType = SemanticModel.Compilation.GetTypeByMetadataName(containingType.ToDisplayString() + "+Child");

            if (childType != null)
            {
                var parentConstructor = symbol;
                var childConstructor = childType.Constructors.FirstOrDefault(c => ParametersMatch(parentConstructor.Parameters, c.Parameters));

                if (childConstructor != null)
                {
                    var newObjectCreation = SyntaxFactory.ObjectCreationExpression(
                        SyntaxFactory.IdentifierName(childType.Name),
                        node.ArgumentList,
                        null);

                    return newObjectCreation.WithTriviaFrom(node);
                }
            }
        }

        return base.VisitObjectCreationExpression(node);
    }

    private bool ParametersMatch(ImmutableArray<IParameterSymbol> parentParameters, ImmutableArray<IParameterSymbol> childParameters)
    {
        if (parentParameters.Length != childParameters.Length)
            return false;

        for (int i = 0; i < parentParameters.Length; i++)
        {
            if (!parentParameters[i].Type.Equals(childParameters[i].Type))
                return false;
        }

        return true;
    }
}