namespace MutationEngine.Abstractions;

public interface IMutator
{
    string ApplyMutation(string originalContent);
    string Name { get; init; }
}