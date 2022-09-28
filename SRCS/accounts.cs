using Logger;
using System;
using SOHash;
using System.IO;
using Base64Var;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Accounts
{
    class AccountHandler
    {

        public static bool Exists(B64[] password)
        {
            Account[] accounts = JArray.Parse(File.ReadAllText(@"USER/accounts.json")).ToObject<Account[]>();
            
            return accounts.Any(item => item.hash == Hash.HString(B64Convert.B64ArrayToString(password)));
        }
        public static void Generate()
        {
            if (!Directory.Exists(@"USER"))
            {
                Directory.CreateDirectory(@"USER");
            }
            if (!File.Exists(@"USER/accounts.json" ))
            {
                File.WriteAllText(@"USER/accounts.json", "[]");
            }
            try
            {
                JArray.Parse(File.ReadAllText(@"USER/accounts.json"));
            }
            catch(Exception)
            {
                Log.Error("USER/accounts.json is invalid JSON");
                Environment.Exit(1);
            }
        }
        public static Account GetAccount(B64[] password)
        {   
            Account[] accounts = JArray.Parse(File.ReadAllText(@"USER/accounts.json")).ToObject<Account[]>();
            return  accounts.SingleOrDefault(item => item.hash == Hash.HString(B64Convert.B64ArrayToString(password)));
        }
        public static Account GetAccount(int index)
        {   
            Account[] accounts = JArray.Parse(File.ReadAllText(@"USER/accounts.json")).ToObject<Account[]>();
            return  accounts[index];
        }
        public static void MakeAccount(B64[] password)
        {
            JArray accounts = JArray.Parse(File.ReadAllText(@"USER/accounts.json"));
            Account account = new Account();
            account.creationdate = DateTime.Now.ToString("hh:mm:ss tt");
            account.hash = Hash.HString(B64Convert.B64ArrayToString(password));
            accounts.Add(JObject.FromObject(account)); 
            File.WriteAllText(@"USER/accounts.json", accounts.ToString());
        }

    }
    class Account
    {
        public string hash;
        public string creationdate;
    }

}