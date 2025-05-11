using BackendTestTask.Internal;
using BackendTestTask.ReaderTextFile;

namespace Tests
{
    [TestClass]
    public sealed class JunkFileTests
    {
        [TestMethod]
        public void JunkFile_EqualsInputOutputFilesTest()
        {
            var lineCountInputFile = File.ReadLines(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles", "test.txt")).Count();

            Sorter sorter = new Sorter();

            var result = sorter.Sort("test.txt", "TestFiles", 10);

            result.Wait();

            var lineCountOutputFile = File.ReadLines(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles", "result.txt")).Count();

            Assert.IsTrue(lineCountInputFile == lineCountOutputFile, "Actual number of strings '{0}' differs from expected one '{1}'.", lineCountOutputFile, lineCountInputFile);

            string[] filePaths = Directory.GetFiles(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "TestFiles"));
            foreach (string filePath in filePaths)
            {
                var name = new FileInfo(filePath).Name;
                name = name.ToLower();
                if (name != "test.txt")
                {
                    File.Delete(filePath);
                }
            }
        }
    }
}
