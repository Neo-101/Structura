namespace Structura.UI
{
    public class ScanError
    {
        public string Path { get; set; }
        public string Message { get; set; }

        public ScanError(string path, string message)
        {
            Path = path;
            Message = message;
        }
    }
}
