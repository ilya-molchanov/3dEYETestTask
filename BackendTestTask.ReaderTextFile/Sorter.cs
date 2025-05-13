using BackendTestTask.Internal;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace BackendTestTask.ReaderTextFile
{
    public class Sorter
    {
        private double _previousPercent = 0;

        private long _fileSize = 0;

        private class RowState
        {
            public Row Line { get; set; }

            public StreamReader Reader { get; set; }
        }

        public async Task Sort(string fileName, string folderName, int partLinesCount)
        {
            var timer = new Stopwatch();
            timer.Start();

            _fileSize = new FileInfo(Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, folderName, fileName)).Length;

            Console.WriteLine("Split input file into pieces has been started");
            // Split input file into 100 MB pieces
            var files = SplitFile(fileName, folderName, partLinesCount);

            Console.WriteLine("Sort parts of files has been started");
            // Sort each part
            SortParts(files);
            
            Console.WriteLine("Merge sorted files has been started");
            // Combine the sorted parts
            MergeSortedFilesIntoOutputFile(files, folderName);

            timer.Stop();
            Console.WriteLine(timer.Elapsed.ToString(@"m\:ss\.fff"));
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
                _previousPercent = 0;
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

                    var percent = (int)Math.Round((float)streamReader.BaseStream.Position / streamReader.BaseStream.Length * 100, 0);
                    
                    PercentCalculation(percent, string.Format("{0}% of files split is completed.", percent));
                }
            }

            return list.ToArray();
        }

        /// <summary>
        /// Sorting of an each splitted file
        /// </summary>
        /// <param name="originalFile">Splitted file to sort</param>
        public static void InternalSort(string originalFile)
        {
            var sortedLines = File.ReadAllLines(originalFile)
                    .Select(x => new Row(x))
                    .OrderBy(x => x);

            File.WriteAllLines(originalFile, sortedLines.Select(x => x.Construct()));
        }

        /// <summary>
        /// Sorting of an each splitted file
        /// </summary>
        /// <param name="files">A given array of splitted files to sort</param>
        private void SortParts(string[] files)
        {
            _previousPercent = 0;

            List<Task> tasks = new List<Task>();
            int taskPoolSize = Environment.ProcessorCount;
            var taskFact = new TaskFactory();
            var taskPool = new List<Task>();
            int tasksIterationsCount = files.Length / taskPoolSize;
            ConcurrentQueue<string> cq = new ConcurrentQueue<string>();
            foreach (var file in files)
            {
                cq.Enqueue(file);
            }

            if (files.Length % taskPoolSize > 0)
            {
                tasksIterationsCount++;
            }
            int fileIdentifier = 0;
            for (int i = 0; i < tasksIterationsCount; i++)
            {
                for (var j = 0; j < taskPoolSize; j++)
                {
                    if (cq.Count > 0)
                    {
                        taskPool.Add(taskFact.StartNew(() =>
                        {
                            string localFileString;
                            if (cq.TryDequeue(out localFileString))
                            {
                                InternalSort(localFileString);
                            }
                        }));
                    }
                    else
                        break;
                }
                Task.WaitAll(taskPool.ToArray());
                taskPool.Clear();

                var percent = (int)Math.Round((float)(files.Length - cq.Count) / files.Length * 100, 0);

                PercentCalculation(percent, string.Format("{0}% of sorting parts is completed.", percent));
            }
        }

        /// <summary>
        /// Merge a given array of splitted files into an output file with a final sorting
        /// </summary>
        /// <param name="files">A given array of splitted files to combine and sort</param>
        /// <param name="folderName">A given folder for output file</param>
        private void MergeSortedFilesIntoOutputFile(string[] files, string folderName)
        {
            _previousPercent = 0;

            // Open all files at once and form a reading layer
            var readers = files.Select(x => new StreamReader(x)).ToArray();

            var resultFileName = Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, folderName, "result.txt");

            try
            {
                var lines = readers.Select(x => new RowState
                {
                    Line = new Row(x.ReadLine()),
                    Reader = x
                }).ToList();

                // Create the result file
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

                    var percent = (int)Math.Round((float) writer.BaseStream.Position / _fileSize * 100, 0);

                    PercentCalculation(percent, string.Format("{0}% of merging sorted files is completed.", percent));
                }
            }
            finally
            {
                foreach (var r in readers)
                    r.Dispose();
            }
        }

        private void PercentCalculation(double percent, string message)
        {
            if (percent != _previousPercent)
            {
                Console.WriteLine(message);
                _previousPercent = percent;
            }
        }
    }
}