using System;
using Logger;
using System.IO;
namespace HTTPResponses
{
    class CSSHandler
    {
        public static byte[] GetCSS(string url)
        {
            byte[] returnarr = new byte[2];
            switch ((url.Split("/")[2]))
            {
                case "main.css":
                    returnarr = File.ReadAllBytes(@"HTML/" + url.Split("/")[2]);
                    break;
                case "gamepage.css":
                    returnarr = File.ReadAllBytes(@"HTML/" + url.Split("/")[2]);
                    break;
                case "games.css":
                    returnarr = File.ReadAllBytes(@"HTML/" + url.Split("/")[2]);
                    break;
                default:
                    Log.Error("Client requested a CSS file that does not exist.");
                    break;
            }
            return returnarr;
        }
    }
}