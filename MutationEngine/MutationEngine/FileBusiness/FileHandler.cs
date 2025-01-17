namespace MutationEngine.FileBusiness;

public static class FileHandler
{
    public static string LoadFile(string programPath)
    {
        return File.ReadAllText(programPath);
    }

    public static void SaveFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }
}