using System;
using System.Collections.Generic;
using System.Text;
using Sys = Cosmos.System;
using System.IO;

namespace PinguinDos
{

    public class Kernel : Sys.Kernel
    {
        private void texedit(string filepath)
        {
            bool running = true;
            string[] contentlist = File.ReadAllText(filepath).Split("\n");
            List<string> fcontent = new List<string>(contentlist);
            int[] cindex = new int[] { 0, 0 };
            string[] l = Path.GetFileName(filepath).Split(".");
            int tl = 0;
            if (l[l.Length - 1] != "txt") {
                Console.WriteLine("The file specified was not in .txt format.");
                return;
             }
            //init
            Console.WriteLine("Texedit controls: Esc = exit, CTRL+S = Save");
            Console.WriteLine("Press any key to continue.");
            Console.ReadKey();
            Console.Clear();

            Console.WriteLine($"Texedit v0.0.1 - {Path.GetFileName(filepath)}");
            foreach (string line in fcontent)
            {
                Console.WriteLine(line);
                tl++;  
            }

            while (running){
                var key = Console.ReadKey();
                char keystroke = key.KeyChar;
                Console.TreatControlCAsInput = true;
                Console.WriteLine(keystroke);
                if(key.Key == ConsoleKey.Escape){
                    running = false;
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    fcontent.Insert(cindex[1], "\n");
                    cindex[1]++;
                    tl++;
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                        fcontent[cindex[0]] = fcontent[cindex[0]].Remove(fcontent[cindex[0]].Length - 1);
                    if (cindex[0] > 0) { cindex[0]--; }
                }
                else if (key.Key == ConsoleKey.UpArrow)
                {
                    //moves to last line
                    if (cindex[1] > 0) { cindex[1]--; }
                }
                else if (key.Key == ConsoleKey.DownArrow)
                {
                    //moves to next line
                    if (cindex[1] < tl) { cindex[1]++; }
                }
                else
                {
                    //should handle all other cases (unless delete)
                    fcontent[cindex[0]] += key.ToString();
                }
                foreach (string line in fcontent)
                {
                    Console.WriteLine(line);
                }
                
                
            }
        }
        Sys.FileSystem.CosmosVFS fs = new Sys.FileSystem.CosmosVFS();
        protected override void BeforeRun()
        {
            Sys.FileSystem.VFS.VFSManager.RegisterVFS(fs);
            Console.WriteLine("Pinguin OS v0.0.2");
        }

        protected override void Run()
        {
            

            Console.Write($"{System.IO.Directory.GetCurrentDirectory()}>");
            string cmd = Console.ReadLine();
            bool debug = false;
            /*
            public void parseargs(string cmd)
            {
                string v = Regex.Replace(input:cmd,pattern:" +",replacement:" ");
                v = v.Split(" ")[1].Replace("cmdname");

            }*/

            //Handle cases with trailing data
            if (cmd.Contains(" "))
            {
                string[] cmdarr = cmd.Split(" "); //cd c:/users/... -> ["cd", "c:/users/..."]
                
                if (debug) { Console.WriteLine(cmdarr); }
                switch (cmdarr[0])
                {
                    case "cd":
                        try
                        {
                            if (cmdarr[1].Contains(":"))
                            {
                                Directory.SetCurrentDirectory(cmdarr[1]);
                                Console.WriteLine($"Set directory to {cmdarr[1]}");
                            }
                            else
                            {
                                Console.WriteLine("Directory invalid.");
                            }
                        }catch(DirectoryNotFoundException)
                        {
                            Console.WriteLine("OS cannot find the needed dir.");
                        };
                        break;
                    case "mkf":
                        try
                        {
                            File.Create(cmdarr[1]);
                            Console.WriteLine($"Created file {cmdarr[1]}");
                        }catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;

                    case "mkd":
                        System.IO.Directory.CreateDirectory(cmdarr[1]);
                        Console.WriteLine($"Created Directory {cmdarr[1]}");
                        break;
                    case "prt":
                        try
                        {
                            if (cmdarr[1].Contains(":"))
                            {
                                string content = File.ReadAllText(cmdarr[1]);
                                Console.WriteLine($"File {Path.GetFileName(cmdarr[1])} in {cmdarr[1]} (Approximate size: {content.Length} bytes)");
                                Console.WriteLine(content);
                            }
                            else
                            {
                                string content = File.ReadAllText(System.IO.Directory.GetCurrentDirectory() + cmdarr[1]);
                                Console.WriteLine($"File {Path.GetFileName(cmdarr[1])} in {cmdarr[1]} (Approximate size: {content.Length} bytes)");
                                Console.WriteLine(content);
                            }
                        }catch(Exception e)
                        {
                            Console.WriteLine(e);
                        } 
                        break;
                    case "hex":
                        try
                        {
                            string path = System.IO.Directory.GetCurrentDirectory() + cmdarr[1];
                            FileStream fs = new FileStream(path, FileMode.Open);
                            int hexIn;
                            String hex = "";

                            for (int i = 0; (hexIn = fs.ReadByte()) != -1; i++)
                            {
                                hex = string.Format("{0:X2}", hexIn);
                            }
                            for (int i = 0; i < hex.Length; i++)
                            {
                                if (i > 0 & (i % 2) == 0)
                                {
                                    hex.Insert(i, " ");
                                }
                                if (i > 0 & (i % 48) == 0)
                                {
                                    hex.Insert(i, "\n");
                                }
                            }
                            Console.WriteLine(hex);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        break;
                    case "txedit":
                        texedit("a");
                        break;

                }

            }
            else
            {
                //handle program cases (no trailing data)
                switch (cmd)
                {
                    case "debug":
                        if (debug) { debug = false; } else { debug = true; }
                        Console.WriteLine($"Debug Mode: {debug}");
                        break;
                    case "dspace":
                        string data = $"|Available Free Space: {String.Format("{0:n0}", fs.GetAvailableFreeSpace(System.IO.Directory.GetCurrentDirectory()))} bytes|";
                        string hyphen = new String('-', (data.Length - 2));
                        Console.WriteLine($"+{hyphen}+");
                        Console.WriteLine(data);
                        Console.WriteLine($"+{hyphen}+");
                        break;
                    case "dir":
                        string dir = System.IO.Directory.GetCurrentDirectory();
                        Console.WriteLine(dir);
                        string[] filelist = Directory.GetFiles(dir);
                        if (!(filelist == null || filelist.Length == 0)) { foreach (var file in filelist) { Console.WriteLine(file); } } else { Console.WriteLine("There are no files yet."); }
                        break;
                    case "cls":
                        Console.Clear();
                        break;
                    case "cd":
                        Console.WriteLine(System.IO.Directory.GetCurrentDirectory());
                        break;
                    case "shutdown":
                        Cosmos.System.Power.Shutdown();
                        break;
                    case "reboot":
                        Cosmos.System.Power.Reboot();
                        break;
                    default:
                        Console.WriteLine($"The command '{cmd}' does not exist.");
                        break;
                }
            }
            

        }
    }
}
