using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class OverridingMethodDeletion : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        // Parse the full code into a Syntax Tree
        var syntaxTree = CSharpSyntaxTree.ParseText(code);

        // Get the root node of the syntax tree
        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        if (root == null)
            throw new InvalidOperationException("Failed to parse the code.");

        // Extract all classes from the root node
        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

        // Loop through each class and remove any overridden methods
        var updatedClasses = classes.Select(classNode =>
        {
            var updatedMembers = classNode.Members
                .Where(member =>
                    !(member is MethodDeclarationSyntax method &&
                      method.Modifiers.Any(m => m.IsKind(SyntaxKind.OverrideKeyword))))
                .ToList();

            return classNode.WithMembers(SyntaxFactory.List(updatedMembers));
        }).ToList();

        // Replace the updated classes back into the root
        var updatedRoot = root.ReplaceNodes(classes,
            (oldNode, newNode) =>
                updatedClasses.FirstOrDefault(updatedClass => updatedClass.Identifier.Text == oldNode.Identifier.Text));

        // Return the modified code with overridden methods removed
        return updatedRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "IOD";
}