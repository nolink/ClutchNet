using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutchNet
{
    class Constants
    {
        //注意，Windows下，框架组件需要有如下要求：
        //所有应用必须部署在同一磁盘，暂定D盘，为了与Java处理方式一致，这里使用"/etc/"这种Linux结构
        //最终磁盘结构为：D:/etc/environment 文件标识当前环境
        //D:/etc/clutch 目录存放配置的缓存
        public const string APPID_LITERAL = "appId";
        public const string REF_LITERAL = "config.ref";
        public const string ENV_GROUP = "ENV_GROUP";
        public const string ENV_NAME = "ENV_NAME";
        public const string ENV_LOCATION = "/etc/environment";
        public const string CONFIG_LOCATION = "/etc/clutch";
        public const string CONFIG_ROOT = "/clutch";
        public const string DEV = "DEV";
        public const string PRODUCT = "PRODUCT";
    }
}
