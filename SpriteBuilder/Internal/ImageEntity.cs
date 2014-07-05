namespace SpriteBuilder
{
    public class ImageEntity
    {
        public ImageEntity(string filename, string filePath)
        {
            Filename = filename;
            FilePath = filePath;
        }

        public string FilePath { get; set; }

        public string Filename { get; set; }

    }
}