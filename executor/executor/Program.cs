using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Win32;
using System.IO;
using System.Diagnostics;

namespace executor
{
    class Program
    {
        public static string target = "";
        public static string auth = "";
        public static string arguments = "";
        static void Main(string[] args)
        {
            try
            {
                if (args.Length > 1)
                {
                    auth = args[0];
                    target = args[1];
                    if (args.Length > 2)
                    {
                        arguments = args[2];
                    }else
                    {
                        arguments = "";
                    }
                }else
                {
                    Console.WriteLine("Error: invalid arguments");
                    System.Threading.Thread.Sleep(10000);
                    Environment.Exit(1);
                }
                
                
                

                //Console.WriteLine("Auth: " + auth);
                Console.WriteLine("Target: " + target);
                Console.WriteLine("Arguments: " + arguments);



                FileInfo finfo = new FileInfo(target);

                try
                {
                    string passHash = get_setting("passHash");
                    if (passHash == "")
                    {
                        do_open();
                    }
                    else
                    {
                        try
                        {
                            string s = get_setting("s");
                            if (sha256(s + auth) == passHash)
                            {
                                do_open();
                            }
                            else
                            {
                                Environment.Exit(2);
                            }
                        }
                        catch (Exception ex3)
                        {
                            Console.WriteLine("Error while getting salt: " + ex3.ToString());
                            System.Threading.Thread.Sleep(10000);
                        }
                        
                    }
                }
                catch (Exception ex2)
                {
                    Console.WriteLine("Error while getting passhash: " + ex2.ToString());
                    System.Threading.Thread.Sleep(10000);
                }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                System.Threading.Thread.Sleep(10000);
            }
           
        }

        static void do_open()
        {
            try
            {
                string protected_name = get_setting(target);
                Console.WriteLine("Protected name: " + protected_name);
                File.Copy(target, target + ".unprotected");
                File.Delete(target);
                File.Copy(protected_name, target);
                Process proc = new Process();
                proc.StartInfo.FileName = target;
                if (arguments != "")
                {
                    proc.StartInfo.Arguments = arguments;
                }
                Console.WriteLine("Starting: " + proc.StartInfo.FileName);
                //Thread.Sleep(10000);
                proc.Start();
                proc.WaitForExit();
                File.Delete(target);
                File.Copy(target + ".unprotected", target);
                File.Delete(target + ".unprotected");
                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while copying file: " + ex.ToString());
                System.Threading.Thread.Sleep(10000);
            }
            
        }
        static string sha256(string input)
        {
            try
            {
                System.Security.Cryptography.SHA256Managed crypt = new System.Security.Cryptography.SHA256Managed();
                System.Text.StringBuilder hash = new System.Text.StringBuilder();
                byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(input), 0, Encoding.UTF8.GetByteCount(input));
                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                return hash.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error while hashing input: " + ex.ToString());
                return "";
            }
            
        }
        private static string get_setting(string name)
        {
            try
            {
                return Registry.LocalMachine.OpenSubKey("SOFTWARE", false).OpenSubKey("Semrau Software Consulting", false).OpenSubKey("SuspiciousActBlocker", false).GetValue(name).ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting " + name + " setting: " + ex.ToString());
                return "";
            }
            
        }
    }
}
