using System;
using Logger;
using Config;
using Content;
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
                HttpListenerResponse resp = ctx.Response;

                // Print out some info about t  he request
                Log.Network(req.Url.ToString());
                Log.Network(req.HttpMethod);
                Log.Network(req.UserHostName);
                Log.Network(req.UserAgent);
                Console.WriteLine();

                
              
                
                // Make sure we don't increment the page views counter if `favicon.ico` is requested
                byte[] data;
                bool isimage;
                try {
                    if ((req.Url.AbsolutePath.Remove(8, req.Url.AbsolutePath.Length - 8) == "/Images/"))
                    {
                        isimage = true;
                    }else{
                        isimage=false;
                    }
                }
                catch (Exception)
                {
                    isimage = false;
                }
                if(!isimage)
                {
                    string url = req.Url.AbsolutePath;
                    // Write the response info
                    if(req.Url.AbsolutePath == "/")
                    {
                        url = Cont.homepage;
                    }
                    data = HTMLHandler.GetHTML(url);
                    resp.ContentType = "text/html";
                    resp.ContentEncoding = Encoding.UTF8;
                }else{
                    data = ImageHandler.GetImage(req.Url.AbsolutePath);
                    resp.ContentType = ImageHandler.GetMIME(req.Url.AbsolutePath);//"image/png";
                    
                    
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
            
            Conf.Check();
            Conf.Init();
            Cont.Check();
            Cont.Init();
            url = string.Format("http://localhost:{0}/", Conf.port);
            HTTPUrls.ValidUrls = HTTPUrls.GenUrls();
            Log.Clean(Conf.logcount);
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Log.Success(String.Format("OpenModRepo Listenning on {0}", url));
            
            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}