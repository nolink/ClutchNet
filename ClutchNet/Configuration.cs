using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace ClutchNet
{
    public class Configuration
    {

        private static int appid = 0;
        private static ConfigSource config;

        static Configuration()
        {
            string appIdStr = ConfigurationManager.AppSettings[Constants.APPID_LITERAL];
            if (string.IsNullOrEmpty(appIdStr))
            {
                //允许appid大小写不明感
                appIdStr = ConfigurationManager.AppSettings[Constants.APPID_LITERAL.ToLower()];
            }
            if (!string.IsNullOrEmpty(appIdStr))
            {
                int tmp;
                if (int.TryParse(appIdStr, out tmp))
                {
                    appid = tmp;
                }
            }
            config = new ConfigSource();
            config.AppId = appid;
            config.Ref = ConfigurationManager.AppSettings[Constants.REF_LITERAL];
            config.Running = true;
        }

        public static int GetAppId()
        {
            return appid;
        }

        public static string Get(string key)
        {
            return Get(key, null);
        }

        public static string Get(string key, Action<string, string> callback)
        {
            return config.Get(appid, key, callback);
        }

    }
}
