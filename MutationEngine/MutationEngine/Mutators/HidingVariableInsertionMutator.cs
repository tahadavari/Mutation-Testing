using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class HidingVariableInsertionMutator : IMutator
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

        if (classes.Count < 2)
            throw new InvalidOperationException("There should be at least two classes (parent and child).");

        // Get the parent class (first class in the list) and child class (second class in the list)
        var parentClass = classes[0];
        var childClass = classes[1];

        // Get properties of the parent and child class
        var parentProperties = parentClass.Members.OfType<PropertyDeclarationSyntax>().ToList();
        var childProperties = childClass.Members.OfType<PropertyDeclarationSyntax>().ToList();

        // Find properties in parent class that are not in child class
        var missingProperties = parentProperties
            .Where(parentProp =>
                !childProperties.Any(childProp => childProp.Identifier.Text == parentProp.Identifier.Text))
            .ToList();

        // Add the missing properties to the child class with correct syntax (public, get; set;)
        var updatedChildClass = childClass.WithMembers(SyntaxFactory.List(childClass.Members
            .Concat(missingProperties.Select(property =>
            {
                // Create a new property with the correct syntax for public properties with getters and setters
                return SyntaxFactory.PropertyDeclaration(property.Type, property.Identifier)
                    .WithModifiers(
                        SyntaxFactory.TokenList(
                            SyntaxFactory.Token(SyntaxKind.PublicKeyword))) // Make the property public
                    .WithAccessorList(SyntaxFactory.AccessorList(
                        SyntaxFactory.List(new[]
                        {
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration)
                                .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
                        })))
                    .WithLeadingTrivia(property.GetLeadingTrivia()); // Keep original leading trivia
            }))));

        // Replace the updated child class back into the root
        var updatedRoot = root.ReplaceNode(childClass, updatedChildClass);

        // Return the modified code with missing properties added to the child class
        return updatedRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "IHI";
}