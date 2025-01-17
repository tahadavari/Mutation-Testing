using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators
{
    public partial class HidingVariableDeletionMutator : IMutator
    {
        public string Name { get; init; } = "IHD";

        public string ApplyMutation(string originalContent)
        {
            return Mutant(originalContent);
        }

        private string Mutant(string code)
        {
            // Parse the full code into a Syntax Tree
            var syntaxTree = CSharpSyntaxTree.ParseText(code);
            var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

            if (root == null)
                throw new InvalidOperationException("Failed to parse the code.");

            // Extract all classes from the root node
            var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

            if (classes.Count < 2)
                throw new InvalidOperationException("There should be at least two classes (parent and child).");

            var parentClass = classes[0];
            var childClass = classes[1];

            // Get properties of the parent and child class
            var parentProperties = parentClass.Members.OfType<PropertyDeclarationSyntax>().ToList();
            var childProperties = childClass.Members.OfType<PropertyDeclarationSyntax>().ToList();

            // Find common properties by comparing name and type
            var commonProperties = childProperties
                .Where(childProp => parentProperties
                    .Any(parentProp => childProp.Identifier.Text == parentProp.Identifier.Text &&
                                       childProp.Type.ToString() == parentProp.Type.ToString()))
                .ToList();

            // Get the code of the parent class as a string
            var parentClassStringCode = parentClass.NormalizeWhitespace().ToFullString();

            // Get the code of the child class as a string
            var childClassStringCode = childClass.NormalizeWhitespace().ToFullString();

            // Mutate the child class code by commenting out the common properties
            foreach (var property in commonProperties)
            {
                var propertyString = property.ToString();
                var commentProperty = "// " + propertyString;

                // Replace the original property with the commented version in the child class code
                childClassStringCode = childClassStringCode.Replace(propertyString, commentProperty);
            }

            // Replace the mutated child class back into the root
            var updatedRoot = root.ReplaceNode(childClass,
                SyntaxFactory.ParseCompilationUnit(childClassStringCode).DescendantNodes()
                    .OfType<ClassDeclarationSyntax>().First());

            // Return the modified full code with the mutated child class
            return updatedRoot.NormalizeWhitespace().ToFullString();
        }
    }
}