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
            // ToDo Complete the implementation
            // new SuperKeywordInsertion() 
            // new NewMethodCallWithChildClassType()

            // ToDo Compile error
            // new AccessModifierChangeMutator(),

            // new HidingVariableDeletionMutator(),
            // new HidingVariableInsertionMutator(),
            // new OverridingMethodDeletion(),
            // new SuperKeywordDeletion(),
            // new ExplicitCallOfAParentConstructorDeletion(),
            // new ThisKeywordInsertion()
            // new ThisKeywordDeletion()
        ];

        var totalScore = 0.0;
        var originalContent = FileHandler.LoadFile(programFilePath);

        foreach (var mutator in mutators)
        {
            var mutatedContent = mutator.ApplyMutation(originalContent);

            FileHandler.SaveFile(programFilePath, mutatedContent);

            var result = TestExecutor.ExecuteTestsAndCalculateScore(solutionPath);

            Console.WriteLine($"Mutation: {mutator.Name} - Score: {result.Score}");

            FileHandler.SaveFile(programFilePath, originalContent);
        }

        var averageScore = totalScore / mutators.Count;
        Console.WriteLine($"Average Mutation Score: {averageScore}");
    }
}