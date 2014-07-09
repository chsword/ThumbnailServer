namespace Chsword.ThumbnailServer
{
    class ThumbnailSize
    {
        public ThumbnailSize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public int Height { get; set; }
        public int Width { get; set; }
    }
}