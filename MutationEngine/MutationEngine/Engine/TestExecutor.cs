using System.Diagnostics;
using System.Text.RegularExpressions;
using MutationEngine.Models;

namespace MutationEngine.Engine
{
    public static class TestExecutor
    {
        public static TestExecutionResult ExecuteTestsAndCalculateScore(string solutionPath)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"test {solutionPath}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var process = Process.Start(startInfo);

            // خواندن خروجی استاندارد و خطا
            string output = process.StandardOutput.ReadToEnd();
            string errorOutput = process.StandardError.ReadToEnd();

            process.WaitForExit();

            var result = new TestExecutionResult
            {
                PassedTests = GetTestCount(output, "Passed"),
                FailedTests = GetTestCount(output, "Failed"), 
            };

            return result;
        }

        private static int GetTestCount(string output, string status)
        {
            var regex = new Regex($@"{status}:\s*(\d+)", RegexOptions.IgnoreCase);
            var match = regex.Match(output);

            return match.Success ? int.Parse(match.Groups[1].Value) : 0;
        }
    }
}