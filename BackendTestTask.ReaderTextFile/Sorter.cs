using BackendTestTask.Internal;

namespace BackendTestTask.ReaderTextFile
{
    public class Sorter
    {
        private class RowState
        {
            public Row Line { get; set; }

            public StreamReader Reader { get; set; }
        }

        public async Task Sort(string fileName, string folderName, int partLinesCount)
        {
            // Split input file into 100 MB pieces
            var files = SplitFile(fileName, folderName, partLinesCount);

            // Sort each part
            SortParts(files);

            // Combine the sorted parts
            MergeSortedFilesIntoOutputFile(files, folderName);
        }

        /// <summary>
        /// Splitting a file into pieces of a specified size
        /// </summary>
        /// <param name="fileName">File to split</param>
        /// <param name="folderName">Folder contains a given file</param>
        /// <param name="partLinesCount">The number of lines that should be in each splitted file. 
        /// Default number (1024 * 1024 * 4.5) stands for approximately 100Mb size</param>
        /// <returns>Array of splitted files with appropriate paths</returns>
        private string[] SplitFile(string fileName, string folderName, int partLinesCount)
        {
            var list = new List<string>();

            var filePath = Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, folderName, fileName);

            using (StreamReader streamReader = new StreamReader(filePath))
            {
                int partNumber = 0;
                while (!streamReader.EndOfStream)
                {
                    partNumber++;
                    var partFileName = Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, folderName, string.Format("{0}.txt", partNumber));
                    list.Add(partFileName);

                    using (StreamWriter writer = new StreamWriter(partFileName))
                    {
                        for (int i = 0; i < partLinesCount; i++)
                        {
                            if (streamReader.EndOfStream)
                                break;
                            writer.WriteLine(streamReader.ReadLine());
                        }
                    }
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Sorting of an each splitted file
        /// </summary>
        /// <param name="files">A given array of splitted files to sort.</param>
        private void SortParts(string[] files)
        {
            foreach (var file in files)
            {
                var sortedLines = File.ReadAllLines(file)
                    .Select(x => new Row(x))
                    .OrderBy(x => x);

                File.WriteAllLines(file, sortedLines.Select(x => x.Construct()));
            }
        }

        /// <summary>
        /// Merge a given array of splitted files into an output file with a final sorting
        /// </summary>
        /// <param name="files">A given array of splitted files to combine and sort.</param>
        /// <param name="folderName">A given folder for output file</param>
        private void MergeSortedFilesIntoOutputFile(string[] files, string folderName)
        {
            var readers = files.Select(x => new StreamReader(x)).ToArray();

            var resultFileName = Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, folderName, "result.txt");

            try
            {
                var lines = readers.Select(x => new RowState
                {
                    Line = new Row(x.ReadLine()),
                    Reader = x
                }).ToList();

                using StreamWriter writer = new StreamWriter(resultFileName);

                while (lines.Count > 0)
                {
                    var current = lines.OrderBy(x => x.Line).First();
                    writer.WriteLine(current.Line.Construct());

                    if (current.Reader.EndOfStream)
                    {
                        lines.Remove(current);
                        continue;
                    }

                    current.Line = new Row(current.Reader.ReadLine());
                }
            }
            finally
            {
                foreach (var r in readers)
                    r.Dispose();
            }
        }
    }
}