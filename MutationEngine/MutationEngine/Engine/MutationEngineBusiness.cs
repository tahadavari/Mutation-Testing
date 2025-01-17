using MutationEngine.Abstractions;
using MutationEngine.FileBusiness;
using MutationEngine.Mutators;

namespace MutationEngine.Engine;

public static class MutationEngineBusiness
{
    public static void ApplyMutations(string solutionPath, string programFilePath)
    {
        List<IMutator> mutators =
        [
            new AccessModifierChangeMutator()
        ];

        var totalScore = 0.0;
        var originalContent = FileHandler.LoadFile(programFilePath);

        foreach (var mutator in mutators)
        {
            var mutatedContent = mutator.ApplyMutation(originalContent);

            FileHandler.SaveFile(solutionPath, "Program.cs", mutatedContent);

            var score = TestExecutor.ExecuteTestsAndCalculateScore(solutionPath);

            Console.WriteLine($"Mutation: {mutator.Name} - Score: {score.FailedTests}");

            FileHandler.SaveFile(solutionPath, "Program.cs", originalContent);
        }

        var averageScore = totalScore / mutators.Count;
        Console.WriteLine($"Average Mutation Score: {averageScore}");
    }
}