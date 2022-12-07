using NUnit.Framework;
using static Analyser.TestGenerator;
using Analyser;

namespace Tests
{
    public class Tests
    {

        private string classFolder;
        private string generatedClassFolder;
        [SetUp]
        public async Task SetupAsync()
        {
            generatedClassFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\\..\\..\\ClassesForTest\\GeneratedSources"));
            string testClassesFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\\..\\..\\ClassesForTest"));
            var allTestClasses = new List<string>();
            foreach (string path in Directory.GetFiles(testClassesFolder, "*.cs"))
            {
                allTestClasses.Add(path);
            }

            classFolder = Path.Combine(testClassesFolder, "GeneratedSources");
            Directory.CreateDirectory(classFolder);

            Analyser.TestGenerator generator = new(allTestClasses, classFolder, 3, 3, 3);
            await generator.Generate();
        }
        [Test]
        public void FileContaining()
        {
            
            Assert.True(Directory.GetFiles(generatedClassFolder).Length == 3);
        }

    }
}