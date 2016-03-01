using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace ClutchNet
{
    class ConfigEnv
    {

        private static Regex envGroupReg = new Regex(string.Format(".*?{0}=(.*)", Constants.ENV_GROUP));
        private static Regex envNameReg = new Regex(string.Format(".*?{0}=(.*)", Constants.ENV_NAME));
        private static string envGroup = "";
        private static string envName = "";

        static ConfigEnv()
        {
            string fullPath = Path.GetFullPath(Constants.ENV_LOCATION);
            if (File.Exists(fullPath))
            {
                using (StreamReader sr = new StreamReader(fullPath))
                {
                    string str = sr.ReadLine();
                    while (null != str)
                    {
                        if (string.IsNullOrEmpty(envGroup))
                        {
                            var match = envGroupReg.Match(str);
                            if (match.Success)
                            {
                                envGroup = match.Groups[1].Value;
                            }
                        }
                        if (string.IsNullOrEmpty(envName))
                        {
                            var match = envNameReg.Match(str);
                            if (match.Success)
                            {
                                envName = match.Groups[1].Value;
                            }
                        }
                        str = sr.ReadLine();
                    }
                }
            }
        }

        public static string GetEnvGroup()
        {
            return envGroup;
        }

        public static string GetEnvName()
        {
            return envName;
        }

        public static String Resolve()
        {
            String currentEnv = GetEnvName();
            if (string.IsNullOrEmpty(currentEnv))
            {
                return Constants.DEV;
            }
            return currentEnv;
        }

        public static String ResolveEnvGroup()
        {
            String currentEnv = GetEnvGroup();
            if (string.IsNullOrEmpty(currentEnv))
            {
                return Constants.DEV;
            }
            return currentEnv;
        }

        public static bool IsProduction()
        {
            String currentEnv = Resolve();
            return currentEnv.Equals(Constants.PRODUCT);
        }

        public static bool IsDevelopment()
        {
            String currentEnv = Resolve();
            return currentEnv.Equals(Constants.DEV);
        }

        public static string[] GetUris()
        {
            return IsProduction() ? new string[] { 
                    "http://zk1.s1.fx.dcfservice.com:2379", 
                    "http://zk4.s1.fx.dcfservice.com:2379", 
                    "http://zk5.s1.fx.dcfservice.com:2379" } : new string[] { 
                "http://es2.s1.np.fx.dcfservice.com:2379", 
                "http://es4.s1.np.fx.dcfservice.com:2379", 
                "http://es5.s1.np.fx.dcfservice.com:2379" };
        }

    }
}
