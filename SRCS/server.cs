using System;
using Logger;
using Config;
using Content;
using Accounts;
using System.IO;
using System.Net;
using System.Text;
using HTTPResponses;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;


namespace OpenModRepo
{

    class HttpServer
    {
        public static HttpListener listener;
        public static string url;
        public static HttpListenerResponse resp;



        public static async Task HandleIncomingConnections()
        {
            // if for any reason in the future we need to allow users to shutdown the server the bool runServer can be used
            bool runServer = true;

            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();

                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                resp = ctx.Response;
                // Print out some info about t  he request
                Log.Network(req.Url.ToString());
                Log.Network(req.HttpMethod);
                Log.Network(req.UserHostName);
                Log.Network(req.UserAgent);



                byte[] data;
                switch (req.Url.AbsolutePath.Split('/')[1])
                {
                    case "Images":
                        data = ImageHandler.GetImage(req.Url.AbsolutePath);
                        resp.ContentType = ImageHandler.GetMIME(req.Url.AbsolutePath);
                        break;
                    case "css":
                        data = CSSHandler.GetCSS(req.Url.AbsolutePath);
                        resp.ContentType = "text/css";
                        break;
                    default :
                        string url = req.Url.AbsolutePath;
                        // Write the response info
                        if (req.Url.AbsolutePath == "/")
                        {
                            url = Cont.homepage;
                        }
                        data = HTMLHandler.GetHTML(req, url);
                        resp.ContentType = "text/html";
                        resp.ContentEncoding = Encoding.UTF8;
                        break;
                }
                resp.ContentLength64 = data.LongLength;
                resp.StatusCode = HTTPStatus.GetCode(req.Url.AbsolutePath);
                // Write out to the response stream (asynchronously), then close it
                await resp.OutputStream.WriteAsync(data, 0, data.Length);
                resp.Close();
            }
        }


        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            Conf.Check();
            Conf.Init();
            Cont.Check();
            Cont.Init();

            url = string.Format("http://{0}:{1}/", Conf.customip ? Conf.ip : "*", Conf.port);
            HTTPUrls.ValidUrls = HTTPUrls.GenUrls();
            Log.Clean(Conf.logcount + 1);
            AccountHandler.Generate();
            Log.Success("Loaded Accounts.");
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Log.Success(String.Format("OpenModRepo Listenning on {0}", Conf.customip ? url : "port " + Conf.port));

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
        // while empty at the moment this function will be used to save information when the server shut downs
        static void OnProcessExit(object sender, EventArgs e)
        {
            Log.Info("Shutting Down...");
            AccountHandler.SaveAccounts();
            Log.Success("Saved all accounts!");
        }
    }
}
