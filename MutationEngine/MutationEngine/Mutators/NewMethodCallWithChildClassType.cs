using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MutationEngine.Abstractions;
using System.Linq;

namespace MutationEngine.Mutators;

public class NewMethodCallWithChildClassType : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        Console.WriteLine(Mutate(originalContent));
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(code);
        var root = syntaxTree.GetRoot() as CompilationUnitSyntax;

        if (root == null)
            throw new InvalidOperationException("Failed to parse the code.");

        var classes = root.DescendantNodes().OfType<ClassDeclarationSyntax>().ToList();

        if (classes.Count < 2)
            throw new InvalidOperationException("There should be at least two classes (parent and child).");

        var parentClass = classes[0];
        var childClass = classes[1];

        var parentMembers = parentClass.Members.OfType<MemberDeclarationSyntax>().ToList();
        var childMembers = childClass.Members.OfType<MemberDeclarationSyntax>().ToList();

        var allMembersDeclaredInBoth = parentMembers.All(parentMember =>
            childMembers.Any(childMember => AreMembersEquivalent(parentMember, childMember))) &&
            childMembers.All(childMember =>
            parentMembers.Any(parentMember => AreMembersEquivalent(childMember, parentMember)));

        if (allMembersDeclaredInBoth)
        {
            var rewriter = new ObjectInstantiationRewriter(parentClass.Identifier.Text, childClass.Identifier.Text);
            var newRoot = rewriter.Visit(root);
            return newRoot.NormalizeWhitespace().ToFullString();
        }

        return code;
    }

    private bool AreMembersEquivalent(MemberDeclarationSyntax member1, MemberDeclarationSyntax member2)
    {
        if (member1 is MethodDeclarationSyntax method1 && member2 is MethodDeclarationSyntax method2)
        {
            return method1.Identifier.Text == method2.Identifier.Text &&
                   method1.ReturnType.ToString() == method2.ReturnType.ToString();
        }

        if (member1 is PropertyDeclarationSyntax property1 && member2 is PropertyDeclarationSyntax property2)
        {
            return property1.Identifier.Text == property2.Identifier.Text &&
                   property1.Type.ToString() == property2.Type.ToString();
        }

        return false;
    }

    public string Name { get; init; } = "PNC";

    private class ObjectInstantiationRewriter : CSharpSyntaxRewriter
    {
        private readonly string _parentClassName;
        private readonly string _childClassName;

        public ObjectInstantiationRewriter(string parentClassName, string childClassName)
        {
            _parentClassName = parentClassName;
            _childClassName = childClassName;
        }

        public override SyntaxNode VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var typeName = node.Type.ToString();
            if (typeName == _parentClassName)
            {
                var newType = SyntaxFactory.ParseTypeName(_childClassName).WithTriviaFrom(node.Type);
                return node.WithType(newType);
            }
            else if (typeName == _childClassName)
            {
                var newType = SyntaxFactory.ParseTypeName(_parentClassName).WithTriviaFrom(node.Type);
                return node.WithType(newType);
            }

            return base.VisitObjectCreationExpression(node);
        }
    }
}