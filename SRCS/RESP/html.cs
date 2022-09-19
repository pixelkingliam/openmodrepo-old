using System;
using Logger;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace HTTPResponses
{
    class  HTMLHandler
    {
        public static byte[] GetHTML(string url)
        {
            var games_ = JObject.Parse(File.ReadAllText(@"CONF/content.json"));
            var games = games_["games"].ToObject<JArray>();
            string ReplaceWthener = "";
            string ReplaceFinal = "<tr>";
            int i1 = 0;
            
            string[] indexArray = File.ReadAllLines("HTML/index.html");
            string[] OutputFile = indexArray;
            
            
            if (HTTPUrls.ValidUrls.Contains(url))
                {
                    // this generates the dynamic navbar, the reason why it is generated instead of being made in the .html file is because the server would later on be able to be configured with custom navbar entries
                    // This code has hard coded entries and should be rewritten to be more dynamic
                    switch (url)
                        {
                        
                            case "/games":
                                foreach (string item in OutputFile)
                                {
                                    if (item.Contains("<p>navbargoeshere!!</p>"))
                                    {
                                        OutputFile[i1] = "<li><a href=\"/\">Home</a></li>\n<li><a class=\"activated\" href=\"/games\">Games</a></li>\n";
                                    }
                                    i1++;
                                }
                                i1 = 0;
                                break;
                            
                            case "/":
                                i1 = 0;
                                foreach (string item in OutputFile)
                                {
                                    if (item.Contains("<p>navbargoeshere!!</p>"))
                                    {
                                        OutputFile[i1] = "<li><a class=\"activated\" href=\"/\">Home</a></li>\n<li><a href=\"/games\">Games</a></li>\n";
                                    }
                                    i1++;
                                }
                                i1 = 0;
    
                                break;
    
                        }
                        double title_fontsize = 0;
                        double desc_fontsize = 0;
                        i1 = 0;
                        // it'd be wise to rewrite this as a switch later on
                        if (url == "/games")
                        {
                            // this loop generates game cards for /games    
                            foreach (var item in games)
                            {   
                                // these 2 if statements allow for dynamic resizing of font based on string length, this allows user to input long descriptions/titles and still have all the characters readable
                                if (item["Name"].ToString().Length > 20) // only resize font if it can't fit in the first place
                                {
                                    title_fontsize =  15 - (Convert.ToDouble(item["Name"].ToString().Length) - 20 ) * 0.3528 ;//(Convert.ToDouble(item["Name"].ToString().Length) / 11.6);
                                    
                                }else
                                {
                                    // default font size
                                    title_fontsize = 15;
                                }
                                if (item["Desc"].ToString().Length > 95)
                                {
                                    desc_fontsize =  12 - (Convert.ToDouble(item["Desc"].ToString().Length) - 95 ) * 0.0756302521  ;
                                    
                                }else
                                {
                                    desc_fontsize = 12;
                                }
                                
                                string sauce = "<th>\n<div id=\"gamecard\">\n<h1 style=\"font-size: " + title_fontsize + "px\" class=\"title\">" + item["Name"].ToString() + "</h1>\n<img class=\"Posterimage\" width=150px src=\"/Images/" + item["Poster"].ToString() + "\" alt=\"" + item["Desc"].ToString() + "\"\\>\n <div class=\"description\" style=\" font-size: " + desc_fontsize + "px\" >" + item["Desc"].ToString() + "</div>\n</th>\n";
                                i1++;
                                
                                ReplaceWthener = ReplaceWthener + sauce;;
                                
                                if (i1 == 7)
                                {
                                    ReplaceFinal = ReplaceFinal + ReplaceWthener;
                                    
                                    ReplaceFinal = ReplaceFinal + "</tr>\n<tr>";
                                    ReplaceWthener = ""; 
                                    i1=0;
                                }
                            }
                            ReplaceFinal = ReplaceFinal + ReplaceWthener;
                            ReplaceFinal = ReplaceFinal + "</tr>";
                            int i = 0;
                            foreach (string item in OutputFile)
                            {
                                if (item.Contains("<p>gamesgohere!</p>"))
                                {
                                    OutputFile[i] = ReplaceFinal;
                                }
                                i++;
                            }
                        }else{
                            OutputFile = indexArray;
                        }
                }else
                {
                    // 404 handling, part of this code should be rewritten for the navbar rework
                    OutputFile = File.ReadAllLines(@"HTML/404.html");
                    i1 = 0;
                    foreach (string item in OutputFile)
                    {
                        if (item.Contains("<p>navbargoeshere!!</p>"))
                        {
                            OutputFile[i1] = "<li><a class=\"activated\" href=\"/\">Home</a></li>\n<li><a href=\"/games\">Games</a></li>\n";
                        }
                        i1++;
                    }
                    i1 = 0;
                }
                // return the html file in bytes[]
                return Encoding.UTF8.GetBytes(string.Join("", OutputFile));
        }
    }
}