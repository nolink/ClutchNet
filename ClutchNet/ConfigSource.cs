using EtcdNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClutchNet
{
    class ConfigSource
    {
        //使用文档
        //https://github.com/wangjia184/etcdnet

        private Dictionary<string, string> confs = new Dictionary<string, string>();
        private HashSet<string> watchPaths = new HashSet<string>();
        private Dictionary<string, Action<string, string>> callbacks = new Dictionary<string, Action<string, string>>();
        private static Object lockObj = new Object();
        private static Object watchLockObj = new object();
        private EtcdClientOpitions options;
        private EtcdClient etcdClient;
        private int appid;
        private System.Timers.Timer timer;

        public ConfigSource()
        {
            options = new EtcdClientOpitions() { Urls = ConfigEnv.GetUris() };
            etcdClient = new EtcdClient(options);
            timer = new System.Timers.Timer();
            timer.Elapsed += timer_Elapsed;
            timer.Interval = 5*60 * 1000;
            timer.Enabled = true;
        }

        void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            ConfigDump.Store(AppId, confs);
        }

        public string Ref { get; set; }
        public bool Running { get; set; }
        public int AppId { get { return appid; } set { appid = value; confs = ConfigDump.Retrieve(value) as Dictionary<string, string>; } }

        private async void watch(string watchPath)
        {
            long? waitIndex = null;
            EtcdResponse resp;
            while (Running)
            {
                try
                {
                    // when waitIndex is null, get it from the ModifiedIndex
                    if (!waitIndex.HasValue)
                    {
                        resp = await etcdClient.GetNodeAsync(watchPath, recursive: true);
                        if (resp != null && resp.Node != null)
                        {
                            waitIndex = resp.Node.ModifiedIndex + 1;

                            // and also check the children
                            if (resp.Node.Nodes != null)
                            {
                                foreach (var child in resp.Node.Nodes)
                                {
                                    if (child.ModifiedIndex >= waitIndex.Value)
                                        waitIndex = child.ModifiedIndex + 1;

                                    // child node
                                }
                            }
                        }
                    }

                    // watch the changes
                    resp = await etcdClient.WatchNodeAsync(watchPath, recursive: true, waitIndex: waitIndex);
                    if (resp != null && resp.Node != null)
                    {
                        waitIndex = resp.Node.ModifiedIndex + 1;
                        lock (lockObj)
                        {
                            confs[resp.Node.Key] = resp.Node.Value;
                            if (callbacks.ContainsKey(resp.Node.Key))
                            {
                                callbacks[resp.Node.Key](resp.Node.Key, resp.Node.Value);
                            }
                        }
                    }
                    continue;
                }
                catch (TaskCanceledException)
                {
                    // time out, try again
                }
                catch (EtcdException ee)
                {
                    // reset the waitIndex
                    waitIndex = null;
                }
                catch (EtcdGenericException ege)
                {
                    // etcd returns an error
                }
                catch (Exception ex)
                {
                    // generic error
                }

                if (!Running) return;
                // something went wrong, delay 1 second and try again
                await Task.Delay(1000);
            }
        }

        public string Get(int appId, string key, Action<string, string> callback)
        {
            String env = (ConfigEnv.IsDevelopment() && !string.IsNullOrEmpty(Ref)) ? Ref.ToUpper() : ConfigEnv.Resolve();

            String parentPath = String.Format("{0}/{1}/{2}",
                Constants.CONFIG_ROOT, appId, env);
            String nodePath = String.Format("{0}/{1}/{2}/{3}",
                    Constants.CONFIG_ROOT, appId, env, key);

            // 首先，判断parentPath是不是已经获取过了
            // 如果parent获取过了，但是这个key不存在，那么是新加的，获取一下
            // 如果parent获取过了，key也存在，那么直接从内存中拿值，减少http的请求数
            if (!watchPaths.Contains(parentPath))
            {
                lock (watchLockObj)
                {
                    if (!watchPaths.Contains(parentPath))
                    {
                        var resp = etcdClient.GetNodeAsync(parentPath, recursive: true).Result;
                        if (resp.Node.Nodes != null)
                        {
                            lock (lockObj)
                            {
                                foreach (var node in resp.Node.Nodes)
                                {
                                    confs[node.Key] = node.Value;
                                }
                            }
                        }
                        watchPaths.Add(parentPath);
                        watch(parentPath);
                    }
                }
            }

            if (!confs.ContainsKey(nodePath))
            {
                lock (lockObj)
                {
                    if (!confs.ContainsKey(nodePath))
                    {
                        var resp = etcdClient.GetNodeValueAsync(nodePath, ignoreKeyNotFoundException: true).Result;
                        confs[nodePath] = resp;
                    }
                }
            }

            if (callback != null && !callbacks.ContainsKey(nodePath))
            {
                lock (lockObj)
                {
                    if (callback != null && !callbacks.ContainsKey(nodePath))
                    {
                        callbacks.Add(nodePath, callback);
                    }
                }
            }

            return confs[nodePath];

        }

    }
}
