using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public class SuperKeywordInsertion : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        Console.WriteLine(Mutate(originalContent));
        return originalContent;
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

            // Get methods of the parent and child class
            var parentMethods = parentClass.Members.OfType<MethodDeclarationSyntax>().ToList();
            var childMethods = childClass.Members.OfType<MethodDeclarationSyntax>().ToList();

            // Iterate through each method in the child class
            var updatedChildMethods = childMethods.Select(method =>
            {
                // Check if the method accesses any property of the parent class that exists in the child
                var newBody = method.Body?.Statements.Select(statement =>
                {
                    if (statement is ExpressionStatementSyntax expressionStatement)
                    {
                        // Replace child property accesses with base.<property>
                        var updatedExpression = expressionStatement.Expression.ToString();
                        foreach (var parentProperty in parentProperties)
                        {
                            if (updatedExpression.Contains(parentProperty.Identifier.Text))
                            {
                                updatedExpression = updatedExpression.Replace(parentProperty.Identifier.Text, $"base.{parentProperty.Identifier.Text}");
                            }
                        }
                        return SyntaxFactory.ExpressionStatement(SyntaxFactory.ParseExpression(updatedExpression));
                    }
                    return statement;
                }).ToList();

                return method.WithBody(SyntaxFactory.Block(newBody ?? Enumerable.Empty<StatementSyntax>()));
            }).ToList();

            // Replace the updated methods in the child class
            var updatedChildClass = childClass.WithMembers(SyntaxFactory.List(childClass.Members.Concat(updatedChildMethods)));

            // Replace the updated child class back into the root
            var updatedRoot = root.ReplaceNode(childClass, updatedChildClass);

            // Return the modified code with 'base' used in appropriate places
            return updatedRoot.NormalizeWhitespace().ToFullString();
    }

    public string Name { get; init; } = "ISI";
}