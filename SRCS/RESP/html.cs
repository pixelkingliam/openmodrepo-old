using System;
using Logger;
using Content;
using System.IO;
using System.Net;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

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
            
            BuildNavBar(OutputFile, url);
            if (HTTPUrls.ValidUrls.Contains(url))
                {
                        double title_fontsize = 0;
                        double desc_fontsize = 0;
                        i1 = 0;
                        // it'd be wise to rewrite this as a switch later on
                        switch ((url.Split("/")[1]))
                        {
                            case "games":
                            // this loop generates game cards for /games    
                                foreach (var item in games)
                                {   
                                    // these 2 if statements allow for dynamic resizing of font based on string length, this allows user to input long descriptions/titles and still have all the characters readable
                                    if (item["Name"].ToString().Length > 20) // only resize font if it can't fit in the first place
                                    {
                                        title_fontsize =  100 * (15 - ((Convert.ToDouble(item["Name"].ToString().Length) - 20 ) * 0.3528 /* obtained via smallest font that fits w/ char count > string Lgnt devided by def font size*/)/1280);

                                    }else
                                    {
                                        // default font size
                                        title_fontsize = 1.171875;
                                    }
                                    if (item["Desc"].ToString().Length > 95)
                                    {
                                        desc_fontsize =  100 * (12 - ((Convert.ToDouble(item["Desc"].ToString().Length) - 95 ) * 0.0756302521))/1280  ;

                                    }else
                                    {
                                        desc_fontsize = 0.9375;
                                    }

                                    string sauce = "<th>\n<a href=\"/game/0\">\n<div  id=\"gamecard\">\n<h1 style=\"font-size: " + title_fontsize + "vw\" class=\"title\">" + item["Name"].ToString() + "</h1>\n<img class=\"Posterimage\" width=11.71vw src=\"/Images/" + item["Poster"].ToString() + "\" alt=\"" + item["Desc"].ToString() + "\"\\>\n <div class=\"description\" style=\" font-size: " + desc_fontsize + "vw\" >" + item["Desc"].ToString() + "</div>\n</th>\n";
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
                                    if (item.Contains("<p>contentgoeshere!</p>"))
                                    {
                                        OutputFile[i] = ReplaceFinal;
                                    }
                                    i++;
                                }
                                break;
                            case "game":
                                
                                ushort game = Convert.ToUInt16(url.Split("/")[2]); 
                                int i2 = 0;
                                OutputFile = File.ReadAllLines(@"HTML/gamepage.html");
                                BuildNavBar(OutputFile, url);
                                foreach (string item in OutputFile)
                                {
                                    if (item.Contains("<h1>topcontgoeshere!!</h1>"))
                                    {
                                        OutputFile[i2] = "<h1>" + Cont.games[game]["Name"].ToString() + "</h1>";
                                    }
                                    if (item.Contains("<p>topcontgoeshere!!</p>"))
                                    {
                                        OutputFile[i2] = "<p>" + Cont.games[game]["LongDesc"].ToString() + "</p>";
                                    }
                                    if (item.Contains("src=\"imagehere!!\""))
                                    {
                                        OutputFile[i2] = "<img " + "src=\"/Images/" + Cont.games[game]["Poster"].ToString() + "\" </img>";
                                    }
                                    if (item.Contains("<p>Catshere!</p>"))
                                    {
                                        List<string> p = new List<string>();
                                        foreach (var item1 in Cont.games[game]["Caterogies"].ToObject<string[]>())
                                        {
                                            double fontsize;
                                            if (item1.Length > 22) // only resize font if it can't fit in the first place
                                            {
                                                fontsize =  100 * ((19 - ((item1.Length - 22 )) * 0.431818182   )/1280);
                                            }else
                                            {
                                                // default font size
                                                fontsize = 1.484375 ;
                                            }
                                            p.Add(string.Format("<a href=\"/game/{0}\" ><div><p style=\" font-size: {1}vw; \" >{2}</p></div></a>", game + "/" + Cont.games[game]["Caterogies"].ToObject<List<string>>().IndexOf(item1), fontsize, item1));

                                        }
                                        OutputFile[i2] = string.Join("\n", p);
                                    }
                                    
                                    
                                    i2++;
                                }
                                break;
                            default:
                                OutputFile = indexArray;
                                break;
                        }
                }
                else
                {
                    // 404 handling, part of this code should be rewritten for the navbar rework
                    OutputFile = File.ReadAllLines(@"HTML/404.html");
                    i1 = 0;
                    BuildNavBar(OutputFile, url);
                    i1 = 0;
                }
                // return the html file in bytes
                return Encoding.UTF8.GetBytes(string.Join("", OutputFile));
                       
        }
        static string[] BuildNavBar(string[] Template, string url )
        {
            int i1 = 0;
            string navbarResults = "";
            foreach(var item in Cont.navbar)
            {
                string Hclass = "\n";
                if(url == item.Link)
                {
                    Hclass = "class=\"activated\"";
                } 
                navbarResults = navbarResults + string.Format("<li><a {0} href=\"{1}\">{2}</a></li>\n", Hclass, item.Link, item.Name);
            }
            foreach (string item in Template)
            {
                if (item.Contains("<p>navbargoeshere!!</p>"))
                {
                    Template[i1] = navbarResults;
                }
                i1++;
            }
            return Template;

        }
    }
    
}