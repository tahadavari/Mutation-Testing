namespace MutationEngine.Models;

public class TestExecutionResult
{
    public int PassedTests { get; set; }
    public int FailedTests { get; set; }
    public double Score => (double)PassedTests / (PassedTests + FailedTests);
}