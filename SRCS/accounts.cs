using Logger;
using System;
using SOHash;
using System.IO;
using Base64Var;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
namespace Accounts
{
    class AccountHandler
    {
        static List<Account> accounts = new List<Account>();

        public static bool Exists(B64[] password)
        {            
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
            accounts = JArray.Parse(File.ReadAllText(@"USER/accounts.json")).ToObject<List<Account>>();
        }
        public static Account GetAccount(B64[] password)
        {   
            return  accounts.SingleOrDefault(item => item.hash == Hash.HString(B64Convert.B64ArrayToString(password)));
        }
        public static Account GetAccount(int index)
        {   
            return accounts[index];
        }
        public static void MakeAccount(B64[] password)
        {
            Account account = new Account();
            account.creationdate = DateTime.Now.ToString("hh:mm:ss tt");
            account.hash = Hash.HString(B64Convert.B64ArrayToString(password));
            accounts.Add(account); 
        }
        public static void SaveAccounts()
        {
            File.WriteAllText(@"USER/accounts.json", JArray.FromObject(accounts).ToString());
        }
    }
    class Account
    {
        public string hash;
        public string creationdate;
    }

}