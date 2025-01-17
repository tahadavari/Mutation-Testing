using System.Text.RegularExpressions;
using MutationEngine.Abstractions;

namespace MutationEngine.Mutators
{
    public partial class HidingVariableDeletionMutator : IMutator
    {
        public string Name { get; init; } = "IHD";

        public string ApplyMutation(string originalContent)
        {
            var mutatedContent = originalContent;

            mutatedContent = HidingVariableRegex().Replace(mutatedContent, match =>
            {
                var className = match.Groups[1].Value;
                var variableName = match.Groups[2].Value;
                var variableType = match.Groups[3].Value;

                return $"class {className} {{ /* int {variableName}; */ }}";
            });

            return mutatedContent;
        }

        [GeneratedRegex(@"class (\w+)\s*{\s*int (\w+);")]
        private static partial Regex HidingVariableRegex();
    }
}