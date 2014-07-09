using System.Collections.Generic;
using System.Linq;

namespace Chsword.ThumbnailServer
{
    public class ThumbnailSetting
    {
        public ThumbnailSetting()
        {
            FixHeight = 10000;
            AllowSizes = new List<ThumbnailSize>();
        }
        /// <summary>
        /// �̶���ʱ�� д���ĸ߶�
        /// </summary>
        internal int FixHeight { get; set; }
        /// <summary>
        /// �洢�ļ�
        /// </summary>
        internal bool EnableStore { get; set; }

        /// <summary>
        /// �������չ��
        /// </summary>
        internal List<string> AllowExtensions { get; set; }

        /// <summary>
        /// �������гߴ�
        /// </summary>
        internal bool AllowAllSizes { get; set; }
        internal List<ThumbnailSize> AllowSizes { get; set; }


        public ThumbnailSetting Store(bool enableStore = true)
        {
            this.EnableStore = enableStore;
            return this;
        }
        public ThumbnailSetting AllSize(bool allowAllSize = true)
        {
            this.AllowAllSizes = allowAllSize;
            return this;
        }


        public ThumbnailSetting Size(params int[] width)
        {
            this.AllowAllSizes = false;
            if (AllowSizes == null)
                this.AllowSizes = new List<ThumbnailSize>();
            this.AllowSizes.AddRange(width.Select(c => new ThumbnailSize(c, c)));
            return this;
        }
        public ThumbnailSetting FixSize(params int[] width)
        {
            this.AllowAllSizes = false;
            if (AllowSizes == null)
                this.AllowSizes = new List<ThumbnailSize>();
            this.AllowSizes.AddRange(width.Select(c => new ThumbnailSize(c, FixHeight)));
            return this;
        }
        public ThumbnailSetting SetFixWidthBaseHeight(int fixWidthBaseHeight)
        {
            if (FixHeight == fixWidthBaseHeight) return this;
            foreach (var size in this.AllowSizes ?? new List<ThumbnailSize>())
            {
                if (size.Height == FixHeight)
                {
                    size.Height = fixWidthBaseHeight;
                }
            }
            
            FixHeight = fixWidthBaseHeight;
            return this;
        }

        internal bool IsCommonSize(PicInfo picInfo)
        {
            return picInfo.Width == picInfo.Height && this.AllowSizes.Any(c => c.Width == picInfo.Width);
        }

        internal bool IsFixSize(PicInfo picInfo)
        {
            return picInfo.Height == FixHeight &&
                   this.AllowSizes.Any(c => c.Width == picInfo.Width && c.Height == FixHeight);
        }
    }
}