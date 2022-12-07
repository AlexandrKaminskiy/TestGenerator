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
            generatedClassFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\\..\\..\\TestClasses\\GeneratedClasses"));
            string testClassesFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\\..\\..\\TestClasses"));
            var allTestClasses = new List<string>();
            foreach (string path in Directory.GetFiles(testClassesFolder, "*.cs"))
            {
                allTestClasses.Add(path);
            }

            classFolder = Path.Combine(testClassesFolder, "GeneratedClasses");
            Directory.CreateDirectory(classFolder);

            Analyser.TestGenerator generator = new(allTestClasses, classFolder, 3, 3, 3);
            await generator.Generate();
        }

        [Test]
        public void Two_classes_one_ns()
        {
            var classCount = Directory.GetFiles(generatedClassFolder)
                .Count(file => file.Contains("Class2"));
            Assert.AreEqual(classCount, 2);
        }

        [Test]
        public void One_class_one_ns()
        {
            string generatedClassFolder = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\\..\\..\\TestClasses\\GeneratedClasses"));
            var classCount = Directory.GetFiles(generatedClassFolder)
                .Count(file => file.Contains("Class1"));
            Assert.AreEqual(classCount, 1);
        }

    }
}