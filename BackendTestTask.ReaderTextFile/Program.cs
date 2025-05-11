using BackendTestTask.Internal;

namespace BackendTestTask.ReaderTextFile
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Sorter sorter = new Sorter();

            await sorter.Sort("abc.txt", "tmp", (int)(1024 * 1024 * 4.5));
        }
    }
}
