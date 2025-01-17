namespace MutationEngine.FileBusiness;

public static  class FileHandler
{
    public static string LoadFile(string programPath)
    {
        return File.ReadAllText(programPath);
    }
    public static void SaveFile(string projectPath, string fileName, string content)
    {
        var filePath = Path.Combine(projectPath, fileName);
        File.WriteAllText(filePath, content);
    }
}