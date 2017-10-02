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

        /* Executor.exe - takes authorization (password), target program, and arguments that the target was started with.
         * Once it has that information, it authenticates, then renames the protection executable, copies the protected file back and executes
         * the real file with the specified arguments (if they exist).
         */

        public static string target = "";
        public static string auth = "";
        public static string arguments = "";
        static void Main(string[] args)
        {
            try
            {
                // If there are arguments, load them into the variable
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
                
                
                // Debugging information - can be viewed from cmd if executing manually
                Console.WriteLine("Target: " + target);
                Console.WriteLine("Arguments: " + arguments);



                FileInfo finfo = new FileInfo(target);

                try
                {
                    // Check if there is a password.  If so - authenticate.
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
                                // Not authorized - exit specifying an error code.
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
                    // Permissions maybe?
                    Console.WriteLine("Error while getting passhash: " + ex2.ToString());
                    System.Threading.Thread.Sleep(10000);
                }
                
            }
            catch (Exception ex)
            {
                // Ya... that probably shouldn't be happening...
                Console.WriteLine(ex.ToString());
                System.Threading.Thread.Sleep(10000);
            }
           
        }

        private static Random random = new Random();
        public static string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static void do_open()
        {
            try
            {
                string rid = RandomString(64);

                // Find what the random protected name is
                string protected_name = get_setting(target);

                // Verify that the target is not already unprotected
                if (!(File.Exists(target + ".unprotected")))
                {
                    // Copy the main executable to a .unprotected extension
                    File.Copy(target, target + ".unprotected");

                    // Delete the main executable so it doesn't get in the way.
                    File.Delete(target);

                    // Copy the protected program back to it's real name
                    File.Copy(protected_name, target);
                }

                string locktxt = "";

                if (File.Exists(target + ".lock"))
                {
                    string existing_lock = File.ReadAllText(target + ".lock");
                    locktxt = existing_lock + rid + ";";
                }else
                {
                    locktxt = rid + ";";
                }

                File.WriteAllText(target + ".lock", locktxt);

                // Execute it (with arguments if it has any).
                Process proc = new Process();
                proc.StartInfo.FileName = target;
                if (arguments != "")
                {
                    proc.StartInfo.Arguments = arguments;
                }
                Console.WriteLine("Starting: " + proc.StartInfo.FileName);
                

                proc.Start();

                // Wait for it to exit, then re-protect the system.
                proc.WaitForExit();
                if (File.Exists(target + ".lock"))
                {
                    string locks = File.ReadAllText(target + ".lock");
                    if (locks == rid + ";")
                    {
                        File.Delete(target);
                        File.Copy(target + ".unprotected", target);
                        File.Delete(target + ".unprotected");
                    }else
                    {
                        locks = locks.Replace(rid + ";", "");
                        File.WriteAllText(target + ".lock", locks);
                    }
                }
                
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
            // Return the hash of the input string.  Make sure to salt it first.
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
            // Yep - I'm lazy.  Just pulls settings from the registry.
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
