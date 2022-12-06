using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Analyser
{
    internal class Analyser
    {
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

    }
}
