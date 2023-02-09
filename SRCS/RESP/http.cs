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

            if (HTTPUrls.ValidUrls.Contains(url))
            {
                code = 200; // OK
            }
            else
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
            arraysize += (jsoncontents["games"].ToObject<JArray>()).Count;
            arraysize += (jsoncontents["images"].ToObject<JArray>()).Count;
            arraysize += (jsoncontents["builtin-urls"].ToObject<JArray>()).Count;
            arraysize += 3; // amount of hard-coded urls such as CSS files
            arraysize += JArray.Parse(File.ReadAllText(@"USER/accounts.json")).Count;
            Log.Info("Loading " + arraysize + " Urls");
            // this section adds all the urls to the array
            urlarray = new string[arraysize];
            int i = 0;
            // add games

            // these loops are ugly i should remake them using for
            //for (;i < JArray.Parse(File.ReadAllText(@"USER/accounts.json")).Count;i++)
            //{
                
            //}
            int iAfterFunc = i;
            for (int i1 = 0; i1 < JArray.Parse(File.ReadAllText(@"USER/accounts.json")).Count; i1++)
            {
                urlarray[i] = "/Images/pfp/" + i1;
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }

            for (int i1 = 0; i1 < jsoncontents["games"].ToObject<JArray>().Count; i1++)
            {
                urlarray[i] = "/game/" + i1;
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }
            // add images
            for (int i1 = 0; i1 < jsoncontents["images"].ToObject<JArray>().Count; i1++)
            {
                urlarray[i] = "/Images/" + jsoncontents["images"].ToObject<JArray>()[i1]["URL"].ToString();
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }
            // add built in urls
            for (int i1 = 0; i1 < jsoncontents["builtin-urls"].ToObject<JArray>().Count; i1++)
            {
                urlarray[i] = jsoncontents["builtin-urls"].ToObject<JArray>()[i1].ToString();
                Log.Success("Loaded " + urlarray[i]);
                i++;
            }
            urlarray[i] = "/css/main.css";
            i++;
            urlarray[i] = "/css/gamepage.css";
            i++;
            urlarray[i] = "/css/games.css";
            i++;
            Log.Success("Loaded /css/*.css");
            // add non-removable urls (styling)

            return urlarray;
        }

    }
}