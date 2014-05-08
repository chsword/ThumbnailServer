using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Chsword.ThumbnailServer
{
    public class ImageHandler : IHttpHandler
    {
        private static int[] WidthArray =
            new[]
            {
                PictureSizeRules.FaceWidthArray, PictureSizeRules.GroupPicWidthArray, PictureSizeRules.ProductPicWidthArray,
                PictureSizeRules.SharePicWidthArray
            }.SelectMany(c => c).Distinct().ToArray();

        private static int[] FixArray =
            new[] { PictureSizeRules.ProductPicFixWidthArray, PictureSizeRules.SharePicFixWidthArray }.SelectMany(c => c)
                .Distinct().Distinct().ToArray();
        private static bool ProcessImage(HttpContext context)
        {
            var url = context.Request.Url.LocalPath;
            var descPicFileName = context.Server.MapPath(url);
            if (File.Exists(descPicFileName)) return true;
            //如果文件名中含有2个点以上，则404
            if (url.Count(c => c == '.') > 2) return false;
            var arrs = url.Split('_');
            if (arrs.Length <= 1) return false;
            var sourcePicPath = string.Join("_", arrs.Take(arrs.Length - 1));
            var sizeWithExtStr = arrs.LastOrDefault();
            if (string.IsNullOrWhiteSpace(sizeWithExtStr)) return false;
            var sizeWithExtArray = sizeWithExtStr.Split('.');
            if (sizeWithExtArray.Length <= 1) return false;
            var sizeArray = sizeWithExtArray[0].Split('X');
            if (sizeArray.Length <= 1) return false;
            int width = 0,
                height = 0;
            if (!int.TryParse(sizeArray[0], out width) || !int.TryParse(sizeArray[1], out height))
            {
                return false;
            }

            if ((width == height && WidthArray.Contains(width)) ||
                (height == PictureSizeRules.FixHeight && FixArray.Contains(width)))
            {
                if (width <= 0 || height <= 0)
                {
                    return false;
                }
                var sourcePicFileName = context.Server.MapPath(sourcePicPath);

                if (!File.Exists(sourcePicFileName))
                {
                    return false;
                }
                ThumbnailHelper.CreateThumbnail(Image.FromFile(sourcePicFileName), descPicFileName, new Size
                {
                    Width = width,
                    Height = height
                });
                return true;
            }
            return false;
        }

        public void ProcessRequest(HttpContext context)
        {
            var result = ProcessImage(context);
            if (result)
            {
                //context.Response.AddHeader("Cache-Control", "max-age=60");
                //context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("U", DateTimeFormatInfo.InvariantInfo));
                //DateTime ifModifiedSince;
                //if (DateTime.TryParse(context.Request.Headers.Get("If-Modified-Since"), out ifModifiedSince))
                //{
                //    if ((DateTime.Now - ifModifiedSince.AddHours(8)).Seconds < 60)
                //    {
                //        context.Response.Status = "304 Not Modified";
                //        context.Response.StatusCode = 304;
                //        return;
                //    }
                //}
                ImageHelper.WriteImage(new HttpContextWrapper(context), context.Server.MapPath(context.Request.Url.LocalPath));
            }
            else
            {
                NotFound(context);
            }
        }

        private void NotFound(HttpContext context)
        {
            if (context == null)
                throw new ArgumentNullException("context");
            context.Response.StatusCode = 404;
            context.Response.StatusDescription = "文件不存在";
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}