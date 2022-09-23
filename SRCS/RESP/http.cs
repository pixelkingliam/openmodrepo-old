using System;
using Logger;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HTTPResponses
{
    class HTTPStatus
    {
        public static int GetCode(string url)
        {
            // if none of the check pass for whatever reason the server should default to 501
            int code = 501; // Not implemented
            
            if(HTTPUrls.ValidUrls.Contains(url))
            {
                code = 200; // OK
            }else
            {
                code = 404; // Not found
            }
            return code;
        }
    }
    class HTTPUrls
    {
        public static string[] ValidUrls;
        public static string[] GenUrls()
        {
            string[] urlarray;
            int arraysize = 0;
            var jsoncontents = JObject.Parse(File.ReadAllText(@"CONF/content.json"));
            // this section of the function calculates how big the array should be
            arraysize = arraysize + (jsoncontents["games"].ToObject<JArray>()).Count;
            arraysize = arraysize + (jsoncontents["images"].ToObject<JArray>()).Count;
            arraysize = arraysize + (jsoncontents["builtin-urls"].ToObject<JArray>()).Count;
            Log.Info("Loading " + arraysize + " Urls");
            // this section adds all the urls to the array
            urlarray = new string[arraysize];
            int i = 0;
            // add games
            while(i < jsoncontents["games"].ToObject<JArray>().Count)
            {
                urlarray[i] = "/game/" + i;
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }
            // add images
            foreach (var item in jsoncontents["images"].ToObject<JArray>())
            {
                urlarray[i] = "/Images/" + item["URL"].ToString();
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }
            // add built in urls
            foreach (var item in jsoncontents["builtin-urls"].ToObject<JArray>())
            {
                urlarray[i] = item.ToString();
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }
            return urlarray;
        }
        
    }
}