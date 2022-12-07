using TestGenerator;
using Analyser;

string src = Path.GetFullPath(
    Path.Combine(
        Directory
        .GetCurrentDirectory(), @"..\\..\\..\\..\\TestProject1\\ClassesForTest"));//fix
string dest = Path.Combine(src, "GeneratedSources");

var classReader = new ClassReader(dest, src);
List<string> files = classReader.fillFiles();

Console.WriteLine("Maximum quantity of load files");
int maxLoad = int.Parse(Console.ReadLine());

Console.WriteLine("Maximum quantity of tasks files");
int maxExecute = int.Parse(Console.ReadLine());

Console.WriteLine("Maximum quantity of write files");
int maxWrite = int.Parse(Console.ReadLine());

Analyser.TestGenerator generator = new Analyser.TestGenerator(files, dest, maxLoad,
    maxExecute, maxWrite);

await generator.Generate();