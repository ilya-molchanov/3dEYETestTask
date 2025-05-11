namespace BackendTestTask.ReaderTextFile
{
    internal struct Row : IComparable<Row>
    {
        private int position;
        private string row;

        public Row(string line)
        {
            position = line.IndexOf(".");
            Number = int.Parse(line.AsSpan(0, position));
            this.row = line;
        }

        public int Number { get; set; }

        public ReadOnlySpan<char> Word => row.AsSpan(position + 2);

        public int CompareTo(Row other)
        {
            int result = Word.CompareTo(other.Word, StringComparison.InvariantCulture);

            if (result != 0)
            {
                return result;
            }
            return Number.CompareTo(other.Number);
        }

        public string Construct() => row;
    }
}
