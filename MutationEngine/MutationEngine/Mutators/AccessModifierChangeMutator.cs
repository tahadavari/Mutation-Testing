using System.Text.RegularExpressions;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public partial class AccessModifierChangeMutator : IMutator
{
    public string Name { get; init; } = "AMC";

    public string ApplyMutation(string originalContent)
    {
        var mutatedContent = originalContent;

        mutatedContent = MyRegex().Replace(mutatedContent, match =>
        {
            var accessModifier = match.Groups[1].Value;
            var declaration = match.Groups[2].Value;

            var newAccessModifier = accessModifier switch
            {
                "public" => "private",
                "protected" => "private",
                _ => "private"
            };

            return $"{newAccessModifier} {declaration}";
        });

        return mutatedContent;
    }

    [GeneratedRegex(@"\b(public|private|protected)\b")]
    private static partial Regex MyRegex();
}