using TestGeneratorLibrary;
public class Program
{
    static async Task Main()
    {
        await Method1();
    }

    public static async Task Method1()
    {
        string filesFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\\..\\..\\..\\TestProject1\\TestClasses"));
        string destFolder = Path.Combine(filesFolder, "GeneratedClasses");
        Directory.CreateDirectory(destFolder);

        var classFiles = new List<string>();
        foreach (string path in Directory.GetFiles(filesFolder, "*.cs"))
        {
            classFiles.Add(path);
        }

        Console.WriteLine("Enter maxFilesToLoadCount: ");
        var maxFilesToLoadCount = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter maxExecuteTasksCount: ");
        var maxExecuteTasksCount = int.Parse(Console.ReadLine());

        Console.WriteLine("Enter maxFilesToWriteCount: ");
        var maxFilesToWriteCount = int.Parse(Console.ReadLine());

        TestGenerator generator = new TestGenerator(classFiles, destFolder, maxFilesToLoadCount,
            maxExecuteTasksCount, maxFilesToWriteCount);
        await generator.Generate();
    }
}