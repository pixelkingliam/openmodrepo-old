using Logger;
using System;
using SOMisc;
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
        //                      pkey, index
        public static Dictionary<string, int> PrivateKeys = new Dictionary<string, int>();
        public static void Check()
        {
            var AccountsJsonPath = @"USER/accounts.json";
            var UserPath = @"USER";
            if (!Directory.Exists(UserPath))
            {
                Directory.CreateDirectory(UserPath);
            }
            else
            if (!File.Exists(AccountsJsonPath))
            {
                File.WriteAllText(AccountsJsonPath, new JArray().ToString());
            }
        }
        public static bool Exists(B64[] password)
        {            
            return accounts.Any(item => item.hash == Hash.HString(B64Convert.B64ArrayToString(password)));
        }
        public static bool Exists(string Pkey)
        {
            return GetIndex(Pkey) == -1 ? false : true;
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
        public static int GetIndex(B64[] password)
        {

            return accounts.FindIndex(item => item.hash == Hash.HString(B64Convert.B64ArrayToString(password)));
        }
        public static int GetIndex(string PKey)
        {
            return PrivateKeys[PKey];
        }
        public static Account GetAccount(B64[] password)
        {   
            return  accounts.SingleOrDefault(item => item.hash == Hash.HString(B64Convert.B64ArrayToString(password)));
        }
        public static Account GetAccount(int index)
        {   
            return accounts[index];
        }
        public static Account GetAccount(string Pkey)
        {   
            return accounts[PrivateKeys[Pkey]];
        }
        public static void MakeAccount(B64[] password)
        {
            Account account = new Account();
            account.creationdate = DateTime.Now.ToString("hh:mm:ss tt");
            account.hash = Hash.HString(B64Convert.B64ArrayToString(password));
            account.username = "DefaultUsername";
            account.location = "";
            accounts.Add(account); 
            Directory.CreateDirectory(@"USER/" + (accounts.Count - 1));
        }
        public static void ReplaceAccount(int index, Account Newacc)
        {
            accounts[index] = Newacc;
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
        public string username;
        public string location;
    }

}