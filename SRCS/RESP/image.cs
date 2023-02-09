using System;
using Logger;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace HTTPResponses
{
    class ImageHandler
    {
        static JArray jsonimages = (JArray)JObject.Parse(File.ReadAllText("CONF/content.json")).SelectToken("images");
        public static byte[] GetImage(string url)
        {

            byte[] resultarray = new byte[3]{52, 48, 52}; // default, ASCII for `404`
            if (url.Split('/')[1] == "Images")
            {                
                if (url.Split('/').Length == 3)
                {       
                    if(jsonimages.Any(item => (string)item["URL"] == url.Split('/')[2]))//Where(item => item is JObject ? ((JObject)item).ContainsKey(url.Split('/')[2]) : false))
                    {
                        resultarray = File.ReadAllBytes((string)jsonimages.SingleOrDefault(item => (string)item["URL"] == url.Split('/')[2])["File"]);
                    }              
                }else if (url.Split('/').Length >= 4)
                {
                    switch (url.Split('/')[2])
                    {
                        case "pfp":
                            if(url.Split('/')[3] is not null)
                            {
                                if(File.Exists(String.Format(@"USER/{0}/pfp", url.Split("/")[3])))
                                {
                                    resultarray = File.ReadAllBytes(String.Format(@"USER/{0}/pfp", url.Split("/")[3]));
                                }
                            }
                            break;
                    }
                }
            }
            // return image file in byte[]
            return resultarray;

        }
        // while not necessary to return on most modern browsers the server should return a MIME type respective of the file
        public static string GetMIME(string url)
        {
            string resultstring = "";
            foreach (var item in jsonimages)
            {
                if (("/Images/" + item["URL"].ToString()) == url)
                {
                    // GetMimeSwitch() with the user-defined type passed on
                    resultstring = GetMimeSwitch(item["Type"].ToString(), item["File"].ToString());
                    break;
                }
            }
            return resultstring;
        }
        
        static string GetMimeSwitch(string ProposedType, string ProposedFile)
        {
            string resultsring = "";
            switch (ProposedType)
            {
                // currently only these file types are supported but more could easily be added
                case "jpeg":
                    resultsring = "image/jpeg";
                    break;
                case "jpg":
                    resultsring = "image/jpeg";
                    break;
                case "png":
                    resultsring = "image/png";
                    break;
                case "webp":
                    resultsring = "image/webp";
                    break;
                case "bmp":
                    resultsring = "image/bmp";
                    break;
                case "auto":
                    // if auto is the type given by the user this will call the function again but with the file extension as the proposed type insteads
                    // note: using magic bytes for the file type might be smarter
                    FileInfo fi = new FileInfo(ProposedFile);
                    resultsring = GetMimeSwitch(fi.Extension.Remove(0, 1), ProposedFile);
                    break;
                default:
                    resultsring = "idfk";
                    break;
            }
            return resultsring;
        }
    }
}