using BackendTestTask.Internal;
using MMLib.RapidPrototyping.Generators;
using System.Text;

namespace BackendTestTask.TestFileGenerator
{
    public partial class TestFileGeneratorForm : Form
    {
        private static Random random = new Random();

        private static WordGenerator generator = new WordGenerator();

        private CancellationTokenSource cancellationTokenSource { get; set; }

        private bool runningApp = false;

        public TestFileGeneratorForm()
        {
            InitializeComponent();
        }

        private void lblIterationsCount_Click(object sender, EventArgs e)
        {
            tbGigabytesCount.Focus();
        }

        private async void btnGenerate_Click(object sender, EventArgs e)
        {
            if (!runningApp)
            {
                runningApp = true;
                progressBar1.Value = 0;
                btnGenerate.Text = "Cancel";
                lblStatus.Text = $"Processing... 0%";
                cancellationTokenSource = new CancellationTokenSource();

                // c# progress - create a Progress<T> instance to report progress updates
                // c# task example iprogress
                var progress = new Progress<int>(percent =>
                {
                    // c# update the progress bar value
                    if (percent < 100)
                    {
                        progressBar1.Value = percent;
                        lblStatus.Text = $"Processing... {percent}%";                        
                    }
                    else
                    {
                        progressBar1.Value = percent;
                        lblStatus.Text = "Done!";
                    }
                });

                // start the asynchronous operation
                // c# async await progress iprogress
                // progressbar async await c#
                await Task.Run(() => ProcessData(progress)); // c# task

                cancellationTokenSource = null;
                Thread.Sleep(500);
                CleanUI();
            }
            else
            {
                cancellationTokenSource.Cancel();
                Thread.Sleep(1000);
                CleanUI();
            }
        }

        private void CleanUI()
        {
            btnGenerate.Text = "Generate Test File";
            tbGigabytesCount.Text = "1";
            runningApp = false;
            progressBar1.Value = 0;
            lblStatus.Text = "";
        }

        private Task ProcessData(IProgress<int> progress)
        {
            int index = 1;
            int totalProcess = 100;
            double previousPercent = 0;

            return Task.Factory.StartNew(() =>
            {
                int gigabyteCount;                
                int.TryParse(tbGigabytesCount.Text, out gigabyteCount);
                if (gigabyteCount == 0)
                    gigabyteCount = 1;
                long desiredLinesForGigabyte = 1024 * 1024 * (47 * gigabyteCount); // 1 Gb of string lines;

                long iterations = desiredLinesForGigabyte;

                var filePath = Path.Combine(ApplicationHelper.TryGetSolutionDirectoryInfo().FullName, "tmp", "input.txt");

                double previousPercent = 0;

                using (StreamWriter writer = new StreamWriter(filePath, true, Encoding.UTF8))
                {
                    for (int i = 0; i < iterations; i++)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                            break;

                        writer.WriteLine(string.Format("{0}. {1}",
                            random.Next(), generator.Next()));

                        var percent = (int)Math.Round(((float)i / iterations) * 100, 0);
                        if (percent != previousPercent)
                        {
                            if (progress != null)
                                progress.Report(percent);
                            previousPercent = percent;
                        }
                    }
                }

                if (cancellationTokenSource.IsCancellationRequested)
                    File.Delete(filePath);

            }, cancellationTokenSource.Token);
        }

        private void tbIterationsCount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}
