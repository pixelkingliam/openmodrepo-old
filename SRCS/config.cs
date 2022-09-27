//config.cs borrowed from server-obranu
using Logger;
using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace Config
{
    public class Conf
    {
        // default values are set here in case a new server.json must be generated
        public static string ip = "127.0.0.1";
        public static bool customip = false;
        public static ushort port = 1433;
        public static int logcount = 7;
        public static bool allowillegalfilepath = false;
        //-==-
        //-==-
        public static string builddate = "2022-09-01";
        // This function checks to see if the CONF/server.json file can be used with the program, this is mainly to not try to load the json file if it is invalid, or does not contain proper key-values pair, usually user-error
        public static void Check()
        {   //check to see if CONF/ and CONF/server.json exists, 
            if (!Directory.Exists(@"CONF"))
            {
                Directory.CreateDirectory(@"CONF");
                Create();
            }
            else
            if (!File.Exists(@"CONF/server.json"))
            {
                Create();
            }
            else
            {   //check if the file is valid JSON, if it is then check if the file contains all the proper values , if those checks fail the existing file will be deleted and a new one will be made
                try
                {
                    JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));

                }
                catch (JsonException) //some other exception
                {
                    File.Delete(@"CONF/server.json");
                    Create();
                }
                var jsonserverconf = JObject.Parse(System.IO.File.ReadAllText(@"CONF/server.json"));
                if (
                        // i have no idea how adding the ! operator to all the checks makes this work properly, time to never touch it again!
                        jsonserverconf["server-configs"].ToObject<JObject>().ContainsKey("ip") |
                        jsonserverconf["server-configs"].ToObject<JObject>().ContainsKey("customip") |
                        jsonserverconf["server-configs"].ToObject<JObject>().ContainsKey("port") |
                        jsonserverconf["server-configs"].ToObject<JObject>().ContainsKey("logcount") |
                        jsonserverconf["server-configs"].ToObject<JObject>().ContainsKey("allowillegalfilepath") |
                        jsonserverconf["server-info"].ToObject<JObject>().ContainsKey("builddate")


                    )
                {// newer versions of the server might add more options to the configs, this check makes sure that the config is regenerated whenever the server is updated
                    if (!(jsonserverconf["server-info"].ToObject<JObject>().ContainsKey("builddate") || jsonserverconf["server-info"]["builddate"].ToString() == builddate))
                    {
                        File.Delete(@"CONF/server.json");
                        Create();
                    }
                }
                else
                {
                    File.Delete(@"CONF/server.json");
                    Create();
                }
            }
        }
        // this creates a valid server.json file when it is called 

        public static void Create()
        {
            // file's variable are set to the program's default vars
            var jsonserverconf = new JObject();
            var serverconfigs = new JObject();
            var serverinfo = new JObject();
            serverconfigs["ip"] = ip;
            serverconfigs["customip"] = customip;
            serverconfigs["port"] = port;
            serverconfigs["logcount"] = logcount;
            serverconfigs["allowillegalfilepath"] = allowillegalfilepath;


            serverinfo["builddate"] = builddate;

            jsonserverconf["server-configs"] = serverconfigs;
            jsonserverconf["server-info"] = serverinfo;
            File.WriteAllText(@"CONF/server.json", jsonserverconf.ToString());
        }
        // load from variables from server.json, if this does not run then Create() and Check() serve no purpose
        public static void Init()
        {
            JObject jsonserverconf = JObject.Parse(System.IO.File.ReadAllText("CONF/server.json"));
            ip = (string)jsonserverconf.SelectToken("server-configs.ip");
            customip = (bool)jsonserverconf.SelectToken("server-configs.customip");
            port = (ushort)jsonserverconf.SelectToken("server-configs.port");
            logcount = (int)jsonserverconf.SelectToken("server-configs.logcount");
            allowillegalfilepath = (bool)jsonserverconf.SelectToken("server-configs.allowillegalfilepath");


        }
    }
}