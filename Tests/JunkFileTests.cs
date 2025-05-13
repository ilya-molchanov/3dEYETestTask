using BackendTestTask.Internal;
using BackendTestTask.ReaderTextFile;
using System.Text.RegularExpressions;

namespace Tests
{
    [TestClass]
    public sealed class JunkFileTests
    {
        [TestMethod]
        public void JunkFile_EqualsInputIntermediateOutputFilesTest()
        {
            var linesCountInputFile = File.ReadLines(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles", "input.txt")).Count();

            int partLinesCount = 10000;

            Sorter sorter = new Sorter();

            var result = sorter.Sort("input.txt", "TestFiles", partLinesCount);

            result.Wait();

            int intermediateFilesCount = linesCountInputFile / partLinesCount;
            if (linesCountInputFile % partLinesCount > 0)
            {
                intermediateFilesCount++;
            }

            string[] intermediateFilesPaths = Directory.GetFiles(
                Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles"))
                .Where(path => Regex.IsMatch(Path.GetFileName(path), @"^\d\.txt$")).ToArray();

            int intermediateFilesLinesCount = 0;
            foreach (string intermediateFilePath in intermediateFilesPaths)
            {
                intermediateFilesLinesCount += File.ReadLines(intermediateFilePath).Count();
            }

            var linesCountOutputFile = File.ReadLines(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles", "result.txt")).Count();

            Assert.IsTrue(linesCountInputFile == intermediateFilesLinesCount, "Actual number of strings within intermediate files '{0}' differs from expected one '{1}'.", intermediateFilesLinesCount, linesCountInputFile);

            Assert.IsTrue(linesCountInputFile == linesCountOutputFile, "Actual number of strings in output file '{0}' differs from expected one '{1}'.", linesCountOutputFile, linesCountInputFile);

            foreach (string intermediateFilePath in intermediateFilesPaths)
            {
                File.Delete(intermediateFilePath);
            }
            File.Delete(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles", "result.txt"));
        }
    }
}
