using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks.Dataflow;
using Analyser.GeneratedValuesInfo;
using MethodInfo = Analyser.GeneratedValuesInfo.MethodInfo;

namespace Analyser
{
    public class TestGenerator
    {
        private List<string> sourceFiles;
        private string destFolder;
        private int loadFileBO;
        private int taskCountBO;
        private int writeFileBO;
        ExecutionDataflowBlockOptions loadingFileConcurrencyConstraint;
        ExecutionDataflowBlockOptions generationClassesConcurrencyConstraing;
        ExecutionDataflowBlockOptions writingFilesConcurrencyConstraint;
        ActionBlock<List<GeneratedValuesInfo.FileInfo>> writeToFile;
        public TestGenerator(List<string> _sourceFiles, string _destFolder, int maxFilesToLoadCount, int maxExecuteTasksCount, int maxFilesToWriteCount)
        {
            sourceFiles = _sourceFiles;
            destFolder = _destFolder;
            loadFileBO = maxFilesToLoadCount;
            taskCountBO = maxExecuteTasksCount;
            writeFileBO = maxFilesToWriteCount;
        }
        
        public Task Generate()
        {
            InitPipeLine();
            return writeToFile.Completion;
        }

        private void InitPipeLine()
        {
            loadingFileConcurrencyConstraint = new ExecutionDataflowBlockOptions();
            generationClassesConcurrencyConstraing = new ExecutionDataflowBlockOptions();
            writingFilesConcurrencyConstraint = new ExecutionDataflowBlockOptions();

            loadingFileConcurrencyConstraint.MaxDegreeOfParallelism = loadFileBO;
            generationClassesConcurrencyConstraing.MaxDegreeOfParallelism = taskCountBO;
            writingFilesConcurrencyConstraint.MaxDegreeOfParallelism = writeFileBO;

            var loadClasses = new TransformBlock<string, GeneratedValuesInfo.FileInfo>(new Func<string, Task<GeneratedValuesInfo.FileInfo>>(LoadClasses), loadingFileConcurrencyConstraint);
            var generateTestClasses = new TransformBlock<GeneratedValuesInfo.FileInfo, List<GeneratedValuesInfo.FileInfo>>(new Func<GeneratedValuesInfo.FileInfo, Task<List<GeneratedValuesInfo.FileInfo>>>(GenerateTests), generationClassesConcurrencyConstraing);
            var writeToFile = new ActionBlock<List<GeneratedValuesInfo.FileInfo>>(new Func<List<GeneratedValuesInfo.FileInfo>, Task>(WriteToFile), writingFilesConcurrencyConstraint);

            var linkOptions = new DataflowLinkOptions() { PropagateCompletion = true };
            loadClasses.LinkTo(generateTestClasses, linkOptions);
            generateTestClasses.LinkTo(writeToFile, linkOptions);

            FillContainer(loadClasses);

            loadClasses.Complete();
        }
        private void FillContainer(TransformBlock<string, GeneratedValuesInfo.FileInfo> loadClasses)
        {
            foreach (var sourceFile in sourceFiles)
            {
                loadClasses.Post(sourceFile);
            }
        }

        private async Task<GeneratedValuesInfo.FileInfo> LoadClasses(string sourceFile)
        {
            string content;
            using (var reader = new StreamReader(new FileStream(sourceFile, FileMode.Open)))
            {
                content = await reader.ReadToEndAsync();
            }
            return new GeneratedValuesInfo.FileInfo(sourceFile, content);
        }

        private async Task WriteToFile(List<GeneratedValuesInfo.FileInfo> fileInfo)
        {
            foreach (var fi in fileInfo)
            {
                using var writer = new StreamWriter(
                        new FileStream(Path.Combine(destFolder, fi.Name), FileMode.Create));
                await writer.WriteAsync(fi.InnerData);
            }

        }

        private async Task<List<GeneratedValuesInfo.FileInfo>> GenerateTests(GeneratedValuesInfo.FileInfo fi)
        {
            return await GenerateCode(fi);
        }

        private async Task<List<GeneratedValuesInfo.FileInfo>> GenerateCode(GeneratedValuesInfo.FileInfo fi)
        {
            var root = await CSharpSyntaxTree.ParseText(fi.InnerData).GetRootAsync();
            return GenerateCodeFromTree(root);
        }

        private List<GeneratedValuesInfo.FileInfo> GenerateCodeFromTree(SyntaxNode root)
        {
            var usingDirectives = new List<UsingDirectiveSyntax>(root
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>());
            var namespaces = new List<NamespaceDeclarationSyntax>(root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>());

            var nsInfo = new List<NsInfo>();
            foreach (var ns in namespaces)
            {
                var innerClasses = ns.DescendantNodes().OfType<ClassDeclarationSyntax>();
                var innerNsClasses = new List<ClassInfo>();
                foreach (var innerNsClass in innerClasses)
                {
                    innerNsClasses.Add(new ClassInfo(innerNsClass.Identifier.ToString(), 
                        GetMethods(innerNsClass)));
                }
                nsInfo.Add(new NsInfo(ns.Name.ToString(), innerNsClasses));
            }
            return CodeGenerator.Generate(nsInfo, usingDirectives);
        }

        private List<MethodInfo> GetMethods(ClassDeclarationSyntax innerNsClass)
        {
            var methods = innerNsClass
                .DescendantNodes()
                .OfType<MethodDeclarationSyntax>();
            var result = new List<MethodInfo>();
            foreach (var method in methods)
            {
                result.Add(new MethodInfo(method.Identifier.ToString(), 
                    method.ReturnType, GetParameters(method)));
            }
            return result;
        }

        private List<MethodParams> GetParameters(MethodDeclarationSyntax method)
        {
            return method.ParameterList.Parameters
                .Select(param => new MethodParams(param.Identifier.Value.ToString(), param.Type))
                .ToList();
        }
    }
}
