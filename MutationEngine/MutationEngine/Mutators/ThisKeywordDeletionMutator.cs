using System.Text.RegularExpressions;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public partial class ThisKeywordDeletionMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        const string replacement = @"$1 = $1;";
        var regex = MyRegex();
        var modifiedCode = regex.Replace(code, replacement);
        return modifiedCode;
    }

    public string Name { get; init; } = "JTD";

    [GeneratedRegex(@"this\.(\w+)\s*=\s*\1;")]
    private static partial Regex MyRegex();
}