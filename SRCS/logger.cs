// logger.cs borrowed from server-obranu, this is temporary and will be replaced with it's own logging library


using System;
using System.IO;
using System.Collections.Generic;
namespace Logger
{
    public class Launch
    {
        public static double Time = ((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds()) + 5) / (1000));
    }
    public static class Log
    {
        public static string fn = (@"LOGS/" + DateTimeOffset.Now.ToUnixTimeSeconds() + ".log");
        // These functions are presets for simple logger notification, you can either call Output() yourself or add/use a function here
        public static void Success(string ToBeLogged)
        {
            Output(ToBeLogged, "SUCCESS", ConsoleColor.Green, ConsoleColor.Black);
        }
        public static void Info(string ToBeLogged)
        {
            Output(ToBeLogged, "INFO", ConsoleColor.DarkCyan, ConsoleColor.Black);
        }
        public static void Warning(string ToBeLogged)
        {
            Output(ToBeLogged, "WARNING", ConsoleColor.DarkYellow, ConsoleColor.Black);
        }
        public static void Error(string ToBeLogged)
        {
            Output(ToBeLogged, "ERROR", ConsoleColor.Red, ConsoleColor.White);
        }
        public static void Network(string ToBeLogged)
        {
            Output(ToBeLogged, "NETWORK", ConsoleColor.Magenta, ConsoleColor.White);
        }
        public static void Output(string TBL, string type, ConsoleColor BG, ConsoleColor FG)
        {
            // create text to log and add log text to this session's log file
            string tolog = "[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]-[" + type + "]-" + TBL;
            File.AppendAllText(fn, tolog + Environment.NewLine);
            Console.BackgroundColor = BG;
            Console.ForegroundColor = FG;
            Console.Write("[" + type + "]");
            Console.ResetColor();
            // write the time of when this was logged and the message to log itself
            Console.Write("[" + Math.Round((((Convert.ToDouble(DateTimeOffset.Now.ToUnixTimeMilliseconds())) / (1000)) - Launch.Time), 2) + "s" + "]");
            Console.WriteLine(" " + TBL);
        }
        public static void Clean(int count)
        {
            //make logs folder if it does not exists
            if (!Directory.Exists(@"LOGS"))
            {
                Directory.CreateDirectory(@"LOGS");
            }
            string[] FileList = Directory.GetFiles(@"LOGS");
            List<int> UpdatedList = new List<int>(); ;
            if (FileList.Length > count - 1)
            {
                //get all the log filenames, remove the .log at the end and make them into an int, then add it to the UpdatedList[] list
                foreach (var file in FileList)
                {

                    string tmp = file;
                    tmp = tmp.Substring(0, tmp.Length - 4);
                    tmp = tmp.Remove(0, 5);
                    UpdatedList.Add(Convert.ToInt32(tmp));
                }
            }

            //delete extra log files, this is usually only 1
            UpdatedList.Sort();
            // repeat until log count is to desired amount
            while (UpdatedList.Count > count - 1)
            {
                // delete the oldest log
                File.Delete(@"LOGS/" + UpdatedList[0] + ".log");
                UpdatedList.RemoveAt(0);
            }
            Log.Success("Cleared old log files");
        }
    }
}