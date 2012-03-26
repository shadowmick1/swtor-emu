namespace NodeViewer
{
    using System;
    using System.Configuration;

    public static class Config
    {
        public static string AssetsPath = ".";
        public static string Language = "en-us";
        public static bool LoadPrototypeNodes = false;

        public static void Load()
        {
            string str = ConfigurationManager.AppSettings.Get("AssetsPath");
            if (str != null)
            {
                AssetsPath = str;
            }
            str = ConfigurationManager.AppSettings.Get("Language");
            if (str != null)
            {
                Language = str;
            }
            str = ConfigurationManager.AppSettings.Get("LoadPrototypeNodes");
            if (str != null)
            {
                LoadPrototypeNodes = str.ToUpperInvariant() == "true".ToUpperInvariant();
            }
        }
    }
}

