using System;

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
        public ThumbnailConfig Include(string path, Func<ThumbnailSetting, ThumbnailSetting> setting)
        {
            return this;
        }
    }

    public class ThumbnailSetting
    {
    }
}