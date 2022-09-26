using System;
using Logger;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Content
{
    class navbarEntry
    {
        public string Name;
        public string Link;
    }
    class Cont
    {
        public static string homepage = "";
        public static JArray games = new JArray();
        public static JArray images = new JArray();
        public static navbarEntry[] navbar = { new navbarEntry() { Name = "Home", Link = "/" }, new navbarEntry() { Name = "Games", Link = "/games" } };
        public static string[] builtinurls;
        // This function checks to see if the CONF/content.json file can be used with the program, this is mainly to not try to load the json file if it is invalid, or does not contain proper key-values pair, usually user-error
        public static void Check()
        {   //check to see if CONF/ and CONF/content.json exists, 
            if (!Directory.Exists(@"CONF"))
            {
                Directory.CreateDirectory(@"CONF");
                Create();
            }
            else
            if (!File.Exists(@"CONF/content.json"))
            {
                Create();
            }
            else
            {   //check if the file is valid JSON, if it is then check if the file contains all the proper values , if those checks fail the existing file will be deleted and a new one will be made
                try
                {
                    JObject.Parse(System.IO.File.ReadAllText(@"CONF/content.json"));

                }
                catch (JsonException) //some other exception
                {
                    Log.Error("CONF/content.json is not a valid json file!");
                    Environment.Exit(1);
                }
                var jsonservercont = JObject.Parse(System.IO.File.ReadAllText(@"CONF/content.json"));
                if (
                        jsonservercont.ContainsKey("games") ||
                        jsonservercont.ContainsKey("hostname ") ||
                        jsonservercont.ContainsKey("images") ||
                        jsonservercont.ContainsKey("builtin-urls") ||
                        jsonservercont.ContainsKey("navbar")
                    )
                {
                }
                else
                {
                    File.Delete(@"CONF/content.json");
                    Create();
                }
            }

        }
        // this creates a valid content.json file when it is called, this is important to give users a valid blank slate they can edit, 
        public static void Create()
        {
            var jsonservercont = new JObject();
            //var serverconfigs = new JObject();
            //var serverinfo = new JObject();
            jsonservercont["homepage"] = "";
            jsonservercont["games"] = games;
            jsonservercont["images"] = images;
            jsonservercont["navbar"] = JArray.FromObject(navbar);
            // a blank content.json should contain a full builtin-urls array
            jsonservercont["builtin-urls"] = new JArray() { "/games" };
            File.WriteAllText(@"CONF/content.json", jsonservercont.ToString());
        }
        // load from variables from content.json, if this does not run then Create() and Check() serve no purpose
        public static void Init()
        {
            JObject jsonservercont = JObject.Parse(System.IO.File.ReadAllText("CONF/content.json"));
            homepage = (string)jsonservercont.SelectToken("homepage");
            games = jsonservercont.SelectToken("games").ToObject<JArray>();
            images = jsonservercont.SelectToken("images").ToObject<JArray>();
            navbar = jsonservercont.SelectToken("navbar").ToObject<navbarEntry[]>();
            builtinurls = jsonservercont.SelectToken("builtin-urls").ToObject<string[]>();
        }
    }
}