using SOMisc;
using System;
using Logger;
using Content;
using Accounts;
using System.IO;
using Base64Var;
using System.Net;
using System.Text;
using System.Linq;
using OpenModRepo;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace HTTPResponses
{
    class HTMLHandler
    {
        static Random rnd = new Random();
        private static List<B64[]> validpasswords = new List<B64[]>();
        public static byte[] GetHTML(HttpListenerRequest req, string url)
        {
            var games_ = JObject.Parse(File.ReadAllText(@"CONF/content.json"));
            var games = games_["games"].ToObject<JArray>();
            string ReplaceWthener = "";
            string ReplaceFinal = "<tr>";
            int i1 = 0;



            MemoryStream memstream = new MemoryStream();
            var ass = req.InputStream;
            req.InputStream.CopyTo(memstream);
            byte[] bodycontent8 = memstream.ToArray();
            System.Text.Encoding encoding = req.ContentEncoding;
            string bodycontent = encoding.GetString(bodycontent8);
            string[] indexArray = File.ReadAllLines("HTML/games.html");
            string[] OutputFile = indexArray;
            byte[] tofind = new byte[] { 13, 10, 13, 10 };

            if (req.HttpMethod == "POST")
            {
                if (req.ContentType.Contains("multipart/form-data"))
                {
                    // remove header of multipart form data
                    Array.Copy(bodycontent8, Soarr.SearchBytes(bodycontent8, tofind) + 4, bodycontent8, 0, bodycontent8.Length - Soarr.SearchBytes(bodycontent8, tofind) - 4);

                    // remove footer of multipart form data
                    byte[] Boundary = encoding.GetBytes(req.ContentType.Split(';')[1].Split('=')[1]);
                    Array.Resize(ref bodycontent8, Soarr.SearchBytes(bodycontent8, Boundary) - 4);
                }
            }
            BuildNavBar(OutputFile, url, req);
            if (HTTPUrls.ValidUrls.Contains(url))
            {
                double title_fontsize = 0;
                double desc_fontsize = 0;
                i1 = 0;
                switch (url.Split("/")[1])
                {
                    case "games":
                        // this loop generates game cards for /games    
                        foreach (var item in games)
                        {
                            // these 2 if statements allow for dynamic resizing of font based on string length, this allows user to input long descriptions/titles and still have all the characters readable
                            if (item["Name"].ToString().Length > 20) // only resize font if it can't fit in the first place
                            {
                                title_fontsize = 100 * (15 - ((Convert.ToDouble(item["Name"].ToString().Length) - 20) * 0.3528 /* obtained via smallest font that fits w/ char count > string Lgnt devided by def font size*/) / 1280);

                            }
                            else
                            {
                                // default font size
                                title_fontsize = 1.171875;
                            }
                            if (item["Desc"].ToString().Length > 95)
                            {
                                desc_fontsize = 100 * (12 - ((Convert.ToDouble(item["Desc"].ToString().Length) - 95) * 0.0756302521)) / 1280;

                            }
                            else
                            {
                                desc_fontsize = 0.9375;
                            }

                            string sauce = "<th>\n<a href=\"/game/0\">\n<div  id=\"gamecard\">\n<h1 style=\"font-size: " + title_fontsize + "vw\" class=\"title\">" + item["Name"].ToString() + "</h1>\n<img class=\"Posterimage\" width=11.71vw src=\"/Images/" + item["Poster"].ToString() + "\" alt=\"" + item["Desc"].ToString() + "\"\\>\n <div class=\"description\" style=\" font-size: " + desc_fontsize + "vw\" >" + item["Desc"].ToString() + "</div>\n</th>\n";
                            i1++;

                            ReplaceWthener = ReplaceWthener + sauce; ;

                            if (i1 == 7)
                            {
                                ReplaceFinal = ReplaceFinal + ReplaceWthener;

                                ReplaceFinal = ReplaceFinal + "</tr>\n<tr>";
                                ReplaceWthener = "";
                                i1 = 0;
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
                        BuildNavBar(OutputFile, url, req);
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
                                        fontsize = 100 * ((19 - ((item1.Length - 22)) * 0.431818182) / 1280);
                                    }
                                    else
                                    {
                                        // default font size
                                        fontsize = 1.484375;
                                    }
                                    p.Add(string.Format("<a href=\"/game/{0}\" ><div><p style=\" font-size: {1}vw; \" >{2}</p></div></a>", game + "/" + Cont.games[game]["Caterogies"].ToObject<List<string>>().IndexOf(item1), fontsize, item1));

                                }
                                OutputFile[i2] = string.Join("\n", p);
                            }


                            i2++;
                        }
                        break;
                    case "signup":
                        OutputFile = File.ReadAllLines(@"HTML/signin.html");
                        BuildNavBar(OutputFile, url, req);
                        int i3 = 0;

                        if (req.HttpMethod == "POST")
                        {

                            int pindex = 0;
                            for (; pindex <= validpasswords.Count - 1; pindex++)
                            {
                                string validpw = B64Convert.B64ArrayToString(validpasswords[pindex]).Trim();
                                string givenpw = bodycontent.Split('=')[1].Trim();
                                if (validpw == givenpw)
                                {
                                    validpasswords.Remove(validpasswords[pindex]);
                                    AccountHandler.MakeAccount(B64Convert.StringToB64Array(givenpw));
                                    string privkey = Hash.HString(Convert.ToString(rnd.Next()));
                                    AccountHandler.PrivateKeys.Add(privkey, (AccountHandler.GetIndex(B64Convert.StringToB64Array(givenpw))));
                                    Cookie biscuit = new Cookie("Pkey", privkey);
                                    HttpServer.resp.SetCookie(biscuit);
                                }
                            }

                        }

                        foreach (string item in OutputFile)
                        {
                            if (item.Contains("<p>contentgoeshere!</p>"))
                            {
                                B64[] base64 = (B64Math.RandomArray(12));
                                validpasswords.Add(base64);
                                string output = "<h1>" + B64Convert.B64ArrayToString(base64) + "</h1>\n<form method=\"POST\" enctype=\"text/plain\" action=\"/signup\" >\n<input type=\"hidden\" name=\"pass\" value=\"" + B64Convert.B64ArrayToString(base64) + "\" />\n<input type=\"submit\" value=\"claim\"/>\n</form>";
                                OutputFile[i3] = output;

                            }
                            i3++;
                        }
                        break;
                    
                    case "account":
                        Log.Error(((req.Cookies["Pkey"] == null).ToString()));
                        Log.Error((!AccountHandler.PrivateKeys.ContainsKey(req.Cookies["Pkey"].Value)).ToString());
                        if (req.Cookies["Pkey"] == null || !AccountHandler.PrivateKeys.ContainsKey(req.Cookies["Pkey"].Value))
                        {
                            // this code isn't ran, find out why
                            HttpServer.resp.Redirect("/login");
                            break;
                            Log.Error("klol");
                        }
                        if (url.Split("/").Length >= 2)
                        {
                            break;
                        }
                        switch (url.Split("/")[2])
                        {
                            case "configure":
                                OutputFile = File.ReadAllLines(@"HTML/acc_config.html");
                                BuildNavBar(OutputFile, url, req);
                                if (req.HttpMethod == "GET")
                                {
                                    if (req.Cookies["Pkey"] != null)
                                    {
                                        OutputFile = (string[])string.Format(string.Join("\n", OutputFile), AccountHandler.GetAccount(req.Cookies["Pkey"].Value).username, AccountHandler.GetAccount(req.Cookies["Pkey"].Value).location).Split('\n');
                                    }

                                }
                                if (req.HttpMethod == "POST")
                                {
                                    if (req.Cookies["Pkey"] != null && AccountHandler.PrivateKeys.ContainsKey(req.Cookies["Pkey"].Value))
                                    {
                                        OutputFile = (string[])string.Format(string.Join("\n", OutputFile), AccountHandler.GetAccount(req.Cookies["Pkey"].Value).username, AccountHandler.GetAccount(req.Cookies["Pkey"].Value).location).Split('\n');

                                        string uname = "";
                                        string locat = "";
                                        uname = bodycontent.Split('&')[0].Split('=')[1].Replace('+', ' ');
                                        locat = bodycontent.Split('&')[1].Split('=')[1].Replace('+', ' ');
                                        Account newacc = AccountHandler.GetAccount(req.Cookies["Pkey"].Value);
                                        newacc.username = uname;
                                        newacc.location = locat;
                                        AccountHandler.ReplaceAccount(AccountHandler.GetIndex(req.Cookies["Pkey"].Value), newacc);
                                    }
                                    else
                                    {
                                        HttpServer.resp.Redirect("/login");
                                    }

                                }
                                break;
                            case "uploadpfp":
                                if (req.HttpMethod == "POST")
                                {
                                    if (req.Cookies["Pkey"] != null && AccountHandler.PrivateKeys.ContainsKey(req.Cookies["Pkey"].Value))
                                    {
                                        File.WriteAllBytes("USER/" + AccountHandler.GetIndex(req.Cookies["Pkey"].Value) + "/pfp", bodycontent8);

                                        Account newacc = AccountHandler.GetAccount(req.Cookies["Pkey"].Value);
                                        AccountHandler.ReplaceAccount(AccountHandler.GetIndex(req.Cookies["Pkey"].Value), newacc);
                                    }
                                    else
                                    {
                                        HttpServer.resp.Redirect("/login");
                                    }
                                }
                                HttpServer.resp.Redirect("/account/configure");
                                break;
                            default:
                                break;
                        }
                        break;
                    case "login":
                        OutputFile = File.ReadAllLines(@"HTML/login.html");
                        BuildNavBar(OutputFile, url, req);
                        if (req.HttpMethod == "POST")
                        {
                            string privkey = Hash.HString(Convert.ToString(rnd.Next()));

                            AccountHandler.PrivateKeys.Add(privkey, AccountHandler.GetIndex(B64Convert.StringToB64Array(bodycontent.Split('=')[1])));
                            Cookie biscuit = new Cookie("Pkey", privkey);
                            biscuit.Expires = (DateTime.Now.AddDays(1));
                            HttpServer.resp.SetCookie(biscuit);

                        }
                        break;
                    default:
                        OutputFile = indexArray;
                        break;
                }
            }
            else
            {
                OutputFile = File.ReadAllLines(@"HTML/404.html");
                i1 = 0;
                BuildNavBar(OutputFile, url, req);
                i1 = 0;
            }
            // return the html file in bytes
            return Encoding.UTF8.GetBytes(string.Join("", OutputFile));

        }
        static string[] BuildNavBar(string[] Template, string url, HttpListenerRequest req)
        {
            int i1 = 0;
            string navbarResults = "";
            foreach (var item in Cont.navbar)
            {
                string Hclass = "\n";
                if (url == item.Link)
                {
                    Hclass = "class=\"activated\"";
                }
                navbarResults += string.Format("<li><a {0} href=\"{1}\">{2}</a></li>\n", Hclass, item.Link, item.Name);
                //navbarResults += "<li><a Login href=\"{1}\">Sign-in</a></li>\n";
            }
            foreach (string item in Template)
            {
                if (item.Contains("<p>navbargoeshere!!</p>"))
                {
                    Template[i1] = navbarResults;
                }
                if (item.Contains("<p>logbargoeshere!!</p>"))
                {
                    double username_fontsize = 1.11;
                    try
                    {
                        if (req.Cookies["Pkey"] != null && AccountHandler.Exists(req.Cookies["Pkey"].Value))
                        {
                            if (AccountHandler.GetAccount(req.Cookies["Pkey"].Value).username.Length > 15) // only resize font if it can't fit in the first place
                            {//this method (the one commented out) is not constant for getting vw font size, a new method should preferably be found
                                //username_fontsize = 100 * (23 - (((AccountHandler.GetAccount(req.Cookies["Pkey"].Value).username.Length - 15* 0.608 ))));///1280); /* obtained via smallest font that fits w/ char count > string Lgnt devided by def font size*/
                            }
                            Template[i1] = String.Format("<li class=\"username\"><a href=\"/account\" >{0}</a><img src=\"/Images/pfp/{1}\"></button></li>\n",AccountHandler.GetAccount(req.Cookies["Pkey"].Value).username, AccountHandler.GetIndex(req.Cookies["Pkey"].Value));
                        }
                        else
                        {
                            Template[i1] = "<li><a href=\"/login\">Log-in</a></li>\n<li><a href=\"/signup\">Sign-Up</a></li>\n";
                        }
                    }
                    catch (Exception)
                    {
                        Template[i1] = "<li><a href=\"/login\">Log-in</a></li>\n<li><a href=\"/signup\">Sign-Up</a></li>\n";

                    }
                }

                i1++;
            }
            return Template;

        }
    }

}