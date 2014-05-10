using System.IO;
using System.Web;

namespace Chsword.ThumbnailServer
{
    /// <summary>
    /// 输出图片工具类
    /// </summary>
    internal static class ImageHttpHelper
    {
        /// <summary>
        /// Writes the image using the content type derived from file
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        public static void WriteImage(HttpContextBase context, string filename)
        {
            context.Response.ContentType = GetContentType(Path.GetExtension(filename));
            context.Response.TransmitFile(filename);
        }

        /// <summary>
        /// Writes the image with an overriden extension type (no extension of disc file?)
        /// </summary>
        /// <param name="context">The <see cref="HttpContext"/>.</param>
        /// <param name="filename">The filename.</param>
        /// <param name="extension">The extension.</param>
        public static void WriteImage(HttpContextBase context, string filename, string extension)
        {
            context.Response.ContentType = GetContentType(extension);
            context.Response.TransmitFile(filename);
        }




        /// <summary>
        /// Gets the Response.ContentType from file extension (for images)
        /// </summary>
        /// <param name="extension">The extension.</param>
        /// <returns></returns>
        public static string GetContentType(string extension)
        {
            switch (extension.ToLower())
            {
                case ".jpg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".jpeg":
                    return "image/jpeg";
                case ".gif":
                    return "image/gif";
                case ".bmp":
                    return "image/bmp";
                case ".tiff":
                    return "image/tiff";
                default:
                    return "application/octet-stream";
            }
        }
    }
}