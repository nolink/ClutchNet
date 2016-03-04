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
        //宿主的appId
        private static int HostAppId = 0;
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
                    HostAppId = tmp;
                }
            }
            config = new ConfigSource();
            config.AppId = HostAppId;
            config.Ref = ConfigurationManager.AppSettings[Constants.REF_LITERAL];
            config.Running = true;
        }

        public static int GetAppId()
        {
            return HostAppId;
        }

        public static string GetEnvironmentGroup()
        {
            return ConfigEnv.ResolveEnvGroup();
        }

        public static string GetEnvironment()
        {
            return ConfigEnv.Resolve();
        }

        public static string GetWithAppId(int appId, string key)
        {
            return GetWithAppId(appId, key, null);
        }

        public static string GetWithAppId(int appId, string key, string defaultValue)
        {
            return GetWithAppId(appId, key, defaultValue, null);
        }

        public static string GetWithAppId(int appId, string key, string defaultValue, Action<string, string> callback)
        {
            string result = config.Get(appId, key, callback);
            return string.IsNullOrEmpty(result) ? defaultValue : result;
        }

        public static string Get(string key)
        {
            return Get(key, null);
        }

        public static string Get(string key, string defaultValue)
        {
            return Get(key, defaultValue, null);
        }

        public static string Get(string key, string defaultValue, Action<string, string> callback)
        {
            string result = config.Get(HostAppId, key, callback);
            return string.IsNullOrEmpty(result) ? defaultValue : result;
        }

    }
}
