using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Data
{
    [Serializable]
    public class ServerConfig
    {
        public string dataPath;
        // 최대 동접수, port ...
    }

    public class ConfigManager
    {
        public static ServerConfig Config { get; private set; }

        public static void LoadConfig()
        {
            // 경로는 프로그램이 실행할 때 인자로 받아도 되고
            // 기본적으로 실행파일이랑 같은 위치에 넣어놓는 경우가 많다.
            string text = File.ReadAllText("Config.json");
            Config = Newtonsoft.Json.JsonConvert.DeserializeObject<ServerConfig>(text);
        }
    }
}
