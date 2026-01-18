namespace Structura.Core
{
    public class ExportMetadata
    {
        public string SizeUnit { get; set; } = "Bytes";
        public string Description { get; set; } = "Tree structure export from Structura.";
    }

    public class ExportModel
    {
        public ExportMetadata Metadata { get; set; } = new ExportMetadata();
        public string RootPath { get; set; }
        public DirectoryNode Tree { get; set; }

        public ExportModel(string rootPath, DirectoryNode tree)
        {
            RootPath = rootPath;
            Tree = tree;
        }
    }
}
