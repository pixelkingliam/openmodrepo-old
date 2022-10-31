using System;
using Logger;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace HTTPResponses
{
    class ImageHandler
    {
        static JArray jsonimages = (JArray)JObject.Parse(File.ReadAllText("CONF/content.json")).SelectToken("images");
        public static byte[] GetImage(string url)
        {
            // Mono complains if this is not assigned to something before, in all case this wont be returned
            byte[] resultarray = new byte[2] { 1, 1 };
            foreach (var item in jsonimages)
            {
                if (("/Images/" + item["URL"].ToString()) == url)
                {
                    resultarray = File.ReadAllBytes(item["File"].ToString());
                    break;
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
        // non public function used in GetMIME
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