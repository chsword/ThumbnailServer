using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Chsword.ThumbnailServer
{
    public class ThumbnailConfig
    {
        static ThumbnailConfig Instance { get; set; }
        public static ThumbnailConfig Start()
        {
            if (Instance == null)
            {
                Instance = new ThumbnailConfig();
            }
            return Instance;
        }

        public ThumbnailConfig()
        {
            PathDictionary = new Dictionary<string, ThumbnailSetting>();
        }
        Dictionary<string, ThumbnailSetting> PathDictionary { get; set; }

        public ThumbnailConfig Include(string path, Func<ThumbnailSetting, ThumbnailSetting> setting)
        {
            if (PathDictionary.ContainsKey(path))
                return this;
            PathDictionary.Add(path, setting(new ThumbnailSetting()));
            return this;
        }
    }

    public class ThumbnailSetting
    {
        public ThumbnailSetting()
        {
            FixHeight = 10000;
        }
        /// <summary>
        /// 固定宽时的 写定的高度
        /// </summary>
        public int FixHeight { get; set; }
        /// <summary>
        /// 存储文件
        /// </summary>
        public bool EnableStore { get; set; }

        /// <summary>
        /// 允许的扩展名
        /// </summary>
        public List<string> AllowExtensions { get; set; }

        /// <summary>
        /// 允许所有尺寸
        /// </summary>
        public bool AllowAllSizes { get; set; }
        public List<Size> AllowSizes { get; set; }


        public ThumbnailSetting Store(bool enableStore = true)
        {
            this.EnableStore = enableStore;
            return this;
        }
        public ThumbnailSetting Size(bool allowAllSize = true)
        {
            this.AllowAllSizes = allowAllSize;
            return this;
        }


        public ThumbnailSetting Size(params Size[] sizes)
        {
            this.AllowAllSizes = false;
            if (AllowSizes == null)
                this.AllowSizes = new List<Size>();
            this.AllowSizes.AddRange(sizes);
            return this;
        }
        public ThumbnailSetting FixSize(params int[] width)
        {
            this.AllowAllSizes = false;
            if (AllowSizes == null)
                this.AllowSizes = new List<Size>();
            this.AllowSizes.AddRange(width.Select(c => new Size(c, FixHeight)));
            return this;
        }
        public ThumbnailSetting SetFixHeightBaseLine(int fixHeight)
        {
            FixHeight = fixHeight;
            return this;
        }

    }
}