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
        return code;
    }

    public string Name { get; init; } = "ISI";
}