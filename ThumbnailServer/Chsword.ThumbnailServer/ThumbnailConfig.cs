using System;
using System.Collections.Generic;

namespace Chsword.ThumbnailServer
{
    public class ThumbnailConfig
    {
       internal static ThumbnailConfig Instance { get; set; }
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
        internal Dictionary<string, ThumbnailSetting> PathDictionary { get; set; }

        public ThumbnailConfig Include(string path, Func<ThumbnailSetting, ThumbnailSetting> setting)
        {
            if (PathDictionary.ContainsKey(path))
                return this;
            PathDictionary.Add(path, setting(new ThumbnailSetting()));
            return this;
        }
    }
}