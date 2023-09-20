namespace LMS.Extensions
{
    public static class FileExtensions
    {
        public static bool HasPdfExtension(string filePath)
        {
            return Path.HasExtension(filePath) && Path.GetExtension(filePath).Equals(".pdf", StringComparison.OrdinalIgnoreCase);
        }
    }
}
