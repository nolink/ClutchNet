using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ClutchNet
{
    class ConfigDump
    {

        static ConfigDump()
        {
            string path = Path.GetFullPath(Constants.CONFIG_LOCATION);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static void Store(int appId, Dictionary<string, string> d)
        {
            string filename = Path.GetFullPath(string.Format("{0}/{1}-config.properties", Constants.CONFIG_LOCATION, appId));

            using (FileStream fs = File.OpenWrite(filename))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                int count = 0;
                foreach (var pair in d)
                {
                    if (null != pair.Key && null != pair.Value)
                    {
                        count++;
                    }
                }
                // Put count.
                writer.Write(count);
                // Write pairs.
                foreach (var pair in d)
                {
                    if (null != pair.Key && null != pair.Value)
                    {
                        writer.Write(pair.Key);
                        writer.Write(pair.Value);
                    }
                }
            }
        }
        public static Dictionary<string, string> Retrieve(int appId)
        {
            string filename = Path.GetFullPath(string.Format("{0}/{1}-config.properties", Constants.CONFIG_LOCATION, appId));
            if (File.Exists(filename))
            {
                var result = new Dictionary<string, string>();
                using (FileStream fs = File.OpenRead(filename))
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    // Get count.
                    int count = reader.ReadInt32();
                    // Read in all pairs.
                    for (int i = 0; i < count; i++)
                    {
                        string key = reader.ReadString();
                        string value = reader.ReadString();
                        result[key] = value;
                    }
                }
                return result;
            }
            else
            {
                return new Dictionary<string, string>();
            }
        }
    }
}
