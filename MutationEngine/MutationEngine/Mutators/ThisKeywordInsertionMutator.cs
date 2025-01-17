using System.Text.RegularExpressions;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators;

public partial class ThisKeywordInsertionMutator : IMutator
{
    public string ApplyMutation(string originalContent)
    {
        return Mutate(originalContent);
    }

    private string Mutate(string code)
    {
        const string replacement = @"this.$1 = this.$1;";
        var regex = MyRegex();
        var modifiedCode = regex.Replace(code, replacement);
        return modifiedCode;
    }

    public string Name { get; init; } = "JTI";

    [GeneratedRegex(@"this\.(\w+)\s*=\s*\1;")]
    private static partial Regex MyRegex();
}