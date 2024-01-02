using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace url_shortener.App
{
    public class Config
    {
        public string BASE_URL;
        public string DB_PATH;
    }
    public class AppConf
    {
        public Config Config;
        public AppConf()
        {
            Config = JsonConvert.DeserializeObject<Config>(File.ReadAllText("App/Config.json"));
        }

    }
}


