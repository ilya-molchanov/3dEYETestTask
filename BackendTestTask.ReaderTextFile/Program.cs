using BackendTestTask.Internal;

namespace BackendTestTask.ReaderTextFile
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var filePath = Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "tmp", "abc.txt");

            Sorter sorter = new Sorter();

            await sorter.Sort(filePath, (int)(1024 * 1024 * 4.5));
        }
    }
}
