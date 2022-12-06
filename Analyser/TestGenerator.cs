using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Threading.Tasks.Dataflow;
using TestGeneratorLibrary.GeneratedValuesInfo;
using MethodInfo = TestGeneratorLibrary.GeneratedValuesInfo.MethodInfo;

namespace TestGeneratorLibrary
{
    public class TestGenerator
    {
        private List<string> sourceFiles;
        private string destFolder;
        private ExecutionDataflowBlockOptions loadFileBO;
        private ExecutionDataflowBlockOptions taskCountBO;
        private ExecutionDataflowBlockOptions writeFileBO;

        public TestGenerator(List<string> _sourceFiles, string _destFolder, int maxFilesToLoadCount, int maxExecuteTasksCount, int maxFilesToWriteCount)
        {
            sourceFiles = _sourceFiles;
            destFolder = _destFolder;
            loadFileBO = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxFilesToLoadCount };
            taskCountBO = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxExecuteTasksCount };
            writeFileBO = new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = maxFilesToWriteCount };
        }

        public Task Generate()
        {
            var loadClasses = new TransformBlock<string, CsFileInfo>(new Func<string, Task<CsFileInfo>>(LoadClasses), loadFileBO);
            var generateTestClasses = new TransformBlock<CsFileInfo, List<CsFileInfo>>(new Func<CsFileInfo, Task<List<CsFileInfo>>>(GenerateTests), taskCountBO);
            var writeToFile = new ActionBlock<List<CsFileInfo>>(async input => { await WriteToFile(input); }, writeFileBO);

            var linkOptions = new DataflowLinkOptions() { PropagateCompletion = true };
            loadClasses.LinkTo(generateTestClasses, linkOptions);
            generateTestClasses.LinkTo(writeToFile, linkOptions);

            foreach (var sourceFile in sourceFiles)
            {
                loadClasses.Post(sourceFile);
            }
            loadClasses.Complete();

            return writeToFile.Completion;
        }

        private async Task<CsFileInfo> LoadClasses(string sourceFile)
        {
            string content;
            using (var reader = new StreamReader(new FileStream(sourceFile, FileMode.Open)))
            {
                content = await reader.ReadToEndAsync();
            }
            return new CsFileInfo(sourceFile, content);
        }

        private async Task WriteToFile(List<CsFileInfo> fileInfo)
        {
            foreach (var fi in fileInfo)
            {
                using var writer = new StreamWriter(
                        new FileStream(Path.Combine(destFolder, fi.Name), FileMode.Create));
                await writer.WriteAsync(fi.Content);
            }

        }

        private async Task<List<CsFileInfo>> GenerateTests(CsFileInfo fi)
        {
            return await GenerateCode(fi);
        }

        private async Task<List<CsFileInfo>> GenerateCode(CsFileInfo fi)
        {
            var root = await CSharpSyntaxTree.ParseText(fi.Content).GetRootAsync();
            return GenerateCodeFromTree(root);
        }

        private List<CsFileInfo> GenerateCodeFromTree(SyntaxNode root)
        {
            var usingDirectives = new List<UsingDirectiveSyntax>(root
                .DescendantNodes()
                .OfType<UsingDirectiveSyntax>());
            var namespaces = new List<NamespaceDeclarationSyntax>(root
                .DescendantNodes()
                .OfType<NamespaceDeclarationSyntax>());

            var nsInfo = new List<NameSpaceInfo>();
            foreach (var ns in namespaces)
            {
                var innerClasses = ns.DescendantNodes().OfType<ClassDeclarationSyntax>();
                var innerNsClasses = new List<ClassInfo>();
                foreach (var innerNsClass in innerClasses)
                {
                    innerNsClasses.Add(new ClassInfo(innerNsClass.Identifier.ToString(), 
                        GetMethods(innerNsClass)));
                }
                nsInfo.Add(new NameSpaceInfo(ns.Name.ToString(), innerNsClasses));
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

        private List<MethodParameterInfo> GetParameters(MethodDeclarationSyntax method)
        {
            return method.ParameterList.Parameters
                .Select(param => new MethodParameterInfo(param.Identifier.Value.ToString(), param.Type))
                .ToList();
        }
    }
}
