using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Threading;
using System.Reflection;

namespace CSGO_Advanced_Item_Parser
{
    public class Program
    {
        //Natural sort method
        public static IEnumerable<string> NaturalSort(IEnumerable<string> list)
        {
            int maxLen = list.Select(s => s.Length).Max();
            Func<string, char> PaddingChar = s => char.IsDigit(s[0]) ? ' ' : char.MaxValue;

            return list
                    .Select(s =>
                        new
                        {
                            OrgStr = s,
                            SortStr = Regex.Replace(s, @"(\d+)|(\D+)", m => m.Value.PadLeft(maxLen, PaddingChar(m.Value)))
                        })
                    .OrderBy(x => x.SortStr)
                    .Select(x => x.OrgStr);
        }

        //Getting the CS:GO directory
        private static string getcsgoPath()
        {
            var value = "";
            foreach (var process in Process.GetProcessesByName("csgo"))
            {
                value = Path.GetDirectoryName(process.MainModule.FileName);
            }
            return value.ToString();
        }

        //Creating cool ASCII text :D
        private static void createAscii()
        {
            string a1 = "     ____                             __ _  __    ____           ";
            string a2 = @"   / __ \___  ____ ___  _____  _____/ /| |/ /   / __ \___ _   __";
            string a3 = @"  / /_/ / _ \/ __ `/ / / / _ \/ ___/ __|   /   / / / / _ | | / /";
            string a4 = @" / _, _/  __/ /_/ / /_/ /  __(__  / /_/   |   / /_/ /  __| |/ _ ";
            string a5 = @"/_/ |_|\___/\__, /\__,_/\___/____/\__/_/|_|  /_____/\___/|___(_)";
            string a6 = "";
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (a1.Length / 2)) + "}", a1), Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (a2.Length / 2)) + "}", a2), Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (a3.Length / 2)) + "}", a3), Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (a4.Length / 2)) + "}", a4), Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (a5.Length / 2)) + "}", a5), Console.ForegroundColor = ConsoleColor.DarkRed);
            Console.WriteLine(String.Format("{0," + ((Console.WindowWidth / 2) + (a6.Length / 2)) + "}", a6), Console.ForegroundColor = ConsoleColor.DarkRed);
        }

        //Main
        public static void Main(string[] args)
        {
            //Variable #1 - Lists
            List<string> skinsID = new List<string>();
            List<string> skinsTag = new List<string>();
            List<string> skinsName = new List<string>();
            List<string> skinsName_cdn = new List<string>();
            List<string> skinsName_cdn2 = new List<string>();
            List<string> skinsImage = new List<string>();
            List<string> skinsRarity = new List<string>();
            List<string> skinWpnName = new List<string>();

            //Variable #2 - Regex options
            RegexOptions options = RegexOptions.Multiline;
            RegexOptions options2 = RegexOptions.Multiline | RegexOptions.IgnoreCase;

            //Variable #3 - Strings
            string pattern = @"""([\d]*)""[\s]*{[\s]*""name""[\s]*""(.*)""[\s]*""description_[\w]+""[\s]*""#(.*)""[\s]*""description_[\w]+""[\s]*""#(.*)""";
            string pattern2 = @"}[\s]*""([\d]+)""[\s]*{[\s]*""name""[\s]*""([a-zA-Z0-9_]+)""[\s]*""prefab";

            //Part #1 - First Process
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Console.Title = "CS:GO | Advanced Item Parser v" + String.Format(version.Major.ToString() + "." + version.Minor.ToString());
            createAscii();
            Console.WriteLine("Waiting for CS:GO to open...\n".PadLeft(1), Console.ForegroundColor = ConsoleColor.Yellow, Console.BackgroundColor = ConsoleColor.DarkMagenta);
            Console.ResetColor();

            //Part #2 - Check game is opened
            while (true)
            {
                Process[] processes = Process.GetProcessesByName("csgo");
                if (processes.Length != 0)
                {
                    Console.Clear();
                    createAscii();
                    Console.WriteLine("Process started.. Please wait for process finish.\n".PadLeft(1), Console.ForegroundColor = ConsoleColor.Yellow, Console.BackgroundColor = ConsoleColor.DarkMagenta);
                    Console.ResetColor();
                    break;
                }
            }

            //Variable #4 - Extra Strings
            string _itemsFileLocation = File.ReadAllText(getcsgoPath() + @"\csgo\scripts\items\items_game.txt");
            string _englishFileLocation = File.ReadAllText(getcsgoPath() + @"\csgo\resource\csgo_english.txt");
            string _itemscdnFileLocation = File.ReadAllText(getcsgoPath() + @"\csgo\scripts\items\items_game_cdn.txt");

            //Part #3 - Get Items cdn name, id and Tag
            foreach (Match m in Regex.Matches(_itemsFileLocation, pattern))
            {
                if (!m.Groups[4].Value.Contains("Default"))
                {
                    for (int i = 1; i < Regex.Matches(_itemscdnFileLocation, "_" + m.Groups[2].Value + "=").Count + 1; i++)
                    {
                        skinsTag.Add(m.Groups[4].Value);
                    }
                }
                if (!m.Groups[2].Value.Contains("default"))
                {
                    skinsName_cdn.Add(m.Groups[2].Value);
                }

                if (!m.Groups[2].Value.Contains("default"))
                {
                    for (int i = 1; i < Regex.Matches(_itemscdnFileLocation, "_" + m.Groups[2].Value + "=").Count + 1; i++)
                    {
                        skinsName_cdn2.Add(m.Groups[2].Value);

                    }
                }

                if (m.Groups[1].Value != "0")
                {
                    for (int i = 1; i < Regex.Matches(_itemscdnFileLocation, "_" + m.Groups[2].Value + "=").Count + 1; i++)
                    {
                        skinsID.Add(m.Groups[1].Value);
                    }
                }
            }

            //Part #4 - Get Items tag translations
            foreach (var translation in skinsTag)
            {
                string blankTranslation = null;
                Match m = Regex.Match(_englishFileLocation, @"""" + translation + @""".*""(.*)""", options);
                if (m.Groups[1].Value == string.Empty)
                {
                    blankTranslation = translation;
                    Match m2 = Regex.Match(_englishFileLocation, @"""" + blankTranslation + @""".*""(.*)""", options2);
                    skinsName.Add(m2.Groups[1].Value);

                }
                else
                {
                    skinsName.Add(m.Groups[1].Value);

                }
            }

            //Part #5 - Get Items image links
            foreach (var links in skinsName_cdn)
            {
                string blankLink = null;

                foreach (Match m in Regex.Matches(_itemscdnFileLocation, "_" + links + @"=(.*)", options))
                {
                    if (m.Groups[1].Value == string.Empty)
                    {
                        blankLink = links;

                        Match m2 = Regex.Match(_itemscdnFileLocation, "_" + blankLink + @"=(.*)", options2);
                        skinsImage.Add(m2.Groups[1].Value);

                    }
                    else
                    {
                        skinsImage.Add(m.Groups[1].Value);

                    }
                }
            }

            //Part #6 - Get Items rarity
            foreach (var rarity in skinsName_cdn)
            {
                foreach (Match m in Regex.Matches(_itemsFileLocation, @"""" + rarity + @"""\t\t""(.*)""", options))
                {
                    for (int i = 1; i < Regex.Matches(_itemscdnFileLocation, "_" + rarity + "=").Count + 1; i++)
                    {
                        skinsRarity.Add(m.Groups[1].Value);
                    }
                }
            }

            //Part #7 - Zip all list
            var total = skinsID.Zip(skinsName, (first, second) => first + ": " + second);
            var total2 = total.Zip(skinsImage, (first, second) => first + ": " + second.Trim());
            var total3 = total2.Zip(skinsRarity, (first, second) => first + " (" + second + ")");

            //Part #8- Get Items real name
            foreach (var wpnNames in skinsName_cdn)
            {
                foreach (var total4_h in total3)
                {
                    foreach (Match m in Regex.Matches(total4_h, @"http:..media.steampowered.com.apps.730.icons.econ.default_generated.(.{0,35}" + "_" + wpnNames + "_light_large" + ")"))
                    {
                        if (m.Groups[1].Value == string.Empty)
                        {
                            skinWpnName.Add("??");
                        }
                        else
                        {
                            skinWpnName.Add(m.Groups[1].Value.Replace("_" + wpnNames, "").Replace("_light_large", "").Replace("weapon_", "").ToUpper().Replace("İ", "I").Replace("_", " "));
                        }
                    }
                }

            }

            //Part #9- Zip finish list
            var total4 = total3.Zip(skinWpnName, (first, second) => first + "  | " + second);

            //Part #10 - Create save file text
            StringBuilder saveText = new StringBuilder();
            foreach (var item in total4)
            {
                saveText.AppendLine(item);
            }

            //Part #11 - Get sorted save file text
            List<string> items = new List<string>(saveText.ToString().Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
            var sortedList1 = NaturalSort(items).ToArray();

            //Part #12 - Get Items ID
            StringBuilder itemsID = new StringBuilder();
            foreach (Match m in Regex.Matches(_itemsFileLocation, pattern2, options))
            {
                itemsID.AppendLine(m.Groups[1].Value + ": " + m.Groups[2].Value);
            }

            //Part #13 - Finish process
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Clear();
            saveText.Clear();
            foreach (var item in sortedList1)
            {
                saveText.AppendLine(item);
            }
            File.WriteAllText("items_parsed.txt", saveText.ToString().TrimEnd());
            File.WriteAllText("items_definitions.txt", itemsID.ToString());
            createAscii();
            Console.WriteLine("Process finished. Files saved to " + Directory.GetCurrentDirectory() + " directory.".PadRight(1), Console.ForegroundColor = ConsoleColor.Yellow, Console.BackgroundColor = ConsoleColor.DarkMagenta);
            Console.ResetColor();
            Console.WriteLine("\nPress enter to continue...");
            Console.ReadKey();
        }
    }
}