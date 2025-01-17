using MutationEngine.Engine;

public class Program
{
    public static void Main(string[] args)
    {
        var solutionPath = @"C:\Projects\Mutation\LibraryManagment";
        var programPath = @"C:\Projects\Mutation\LibraryManagment\LibraryManagment\Program.cs";
        MutationEngineBusiness.ApplyMutations(solutionPath, programPath);
    }
}