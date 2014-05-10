using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace Chsword.ThumbnailServer
{
    public class ImageHandler : IHttpHandler
    {
        /// <summary>
        /// 处理图片
        /// </summary>
        /// <param name="context">HttpContextBase</param>
        /// <returns>
        /// true图片正常 false图片异常
        /// </returns>
        private static bool ProcessImage(HttpContextBase context)
        {

            if(context.Request.Url==null)return false;
            var descUrl = context.Request.Url.LocalPath;
            //未设置此路径 ，则直接访问图片
            var setting = GetThumbnailSeting(descUrl);
            if (setting == null) return true;

            var descPath = context.Server.MapPath(descUrl);
            if (setting.EnableStore)
            {
                //允许存储文件的情况
                if (File.Exists(descPath)) return true;
            }
            //如果文件名中含有2个点以上的\dX\x.jpg则404
            var picInfo = GetPicInfo(descUrl);
            if (picInfo == null) return false;
            var sourceUrl = picInfo.SourcePath;
           // int width = picInfo.Width,height = picInfo.Height;
            if (setting.AllowAllSizes || 
                setting.IsCommonSize(picInfo) ||
                setting.IsFixSize(picInfo))
            {
              
                var sourcePath = context.Server.MapPath(sourceUrl);

                if (!File.Exists(sourcePath))
                {
                    return false;
                }
                ThumbnailHelper.CreateThumbnail(Image.FromFile(sourcePath), descPath,
                    new ThumbnailSize(picInfo.Width,picInfo.Height));
                return true;
            }
            return false;
        }

        private static PicInfo GetPicInfo(string url)
        {
            var picInfo = new PicInfo();
            if (url.Count(c => c == '.') > 2) return null;
            var arrs = url.Split('_');
            if (arrs.Length <= 1) return null;
            picInfo.SourcePath = string.Join("_", arrs.Take(arrs.Length - 1));
            var sizeWithExtStr = arrs.LastOrDefault();
            if (string.IsNullOrWhiteSpace(sizeWithExtStr)) return null;
            var sizeWithExtArray = sizeWithExtStr.Split('.');
            if (sizeWithExtArray.Length <= 1) return null;
            var sizeArray = sizeWithExtArray[0].Split('X');
            if (sizeArray.Length <= 1) return null;
            int w, h;
            if (!int.TryParse(sizeArray[0], out w) || !int.TryParse(sizeArray[1], out h))
            {
                return null;
            }
            picInfo.Width = w;
            picInfo.Height = h;
            if (picInfo.Width <= 0 || picInfo.Height <= 0)
            {
                return null;
            }
            return picInfo;
        }

        private static ThumbnailSetting GetThumbnailSeting(string url)
        {
            return ThumbnailConfig.Instance.PathDictionary.FirstOrDefault().Value;
        }

        public void ProcessRequest(HttpContext context)
        {
            var contextWrapper = new HttpContextWrapper(context);
            var result = ProcessImage(contextWrapper);
            if (result)
            {
                #region client cache

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

                #endregion
                ImageHttpHelper.WriteImage(contextWrapper, contextWrapper.Server.MapPath(context.Request.Url.LocalPath));
            }
            else
            {
                NotFound(contextWrapper);
            }
        }

        private void NotFound(HttpContextBase context)
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