using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.IO;
using BossShopMaker.Properties;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Diagnostics;

namespace BossShopMaker
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Startup Folders
            if (!Directory.Exists(Application.StartupPath + @"\shops"))
                Directory.CreateDirectory(Application.StartupPath + @"\shops");
            if (!Directory.Exists(Application.StartupPath + @"\layout"))
                Directory.CreateDirectory(Application.StartupPath + @"\layout");

            string groupsFile = Application.StartupPath + @"\layout\groups.txt";
            string shopSource = Application.StartupPath + @"\shopitems.csv";
            string shopsFolder = Application.StartupPath + @"\shops";

            // Creating Minecraft Item List and Groups
            Dictionary<string, string> Groups = new Dictionary<string, string>();

            using(var reader = new StreamReader(groupsFile))
            {
                string line = string.Empty;
                while((line = reader.ReadLine()) != null)
                {
                    Groups.Add(line.Split(';')[0], $"ShopName: {line.Split(';')[0]}Shop{Environment.NewLine}DisplayName: '&0&lCraft Red Land Shop Menu'{Environment.NewLine}shop:{Environment.NewLine}");
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("########### Creating BossShopPro Shops ###########");

            for(int i = 0; i < Groups.Count; i++)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Creating shop for {Groups.ElementAt(i).Key} Items");

                using (var reader = new StreamReader(shopSource))
                {
                    string itemTemplate = Resources.Both;
                    string line = string.Empty;
                    int itemSlot = 1;

                    while ((line = reader.ReadLine()) != null)
                    {
                        var item = new
                        {
                            group = line.Split(';')[0],
                            material = line.Split(';')[1],
                            name = line.Split(';')[2],
                            operation = line.Split(';')[3],
                            amount = line.Split(';')[4],
                            price = line.Split(';')[5],
                            sell = line.Split(';')[6],
                            slot = line.Split(';')[7]
                        };

                        if(Groups.ElementAt(i).Key == item.group)
                        {
                            try
                            {
                                if (item.operation == "Both")
                                    itemTemplate = Resources.Both;
                                if (item.operation == "Sell")
                                    itemTemplate = Resources.Sell;
                                if (item.operation == "Buy")
                                    itemTemplate = Resources.Buy;

                                Groups[item.group] += itemTemplate
                                    .Replace("{SHOP_ITEM}", item.material)
                                    .Replace("{ITEM_TYPE}", item.material)
                                    .Replace("{ITEM_NAME}", item.name)
                                    .Replace("{BUY_PRICE}", double.Parse(item.price, NumberStyles.Any).ToString("0.00", CultureInfo.InvariantCulture))
                                    .Replace("{SELL_PRICE}", double.Parse(item.sell, NumberStyles.Any).ToString("0.00", CultureInfo.InvariantCulture))
                                    .Replace("{SHOP_SLOT}", (item.slot != "Auto") ? item.slot : itemSlot++.ToString());

                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"\rAdd {item.name} to {Groups.ElementAt(i).Key}Shop on ShopSlot {itemSlot}          ");
                                Thread.Sleep(5);
                            }
                            catch (Exception) { throw; }
                        }
                    }

                    int pageSlot = 54;

                    if(itemSlot <= 45)
                        Groups[Groups.ElementAt(i).Key] += Resources.ReturnButton.Replace("{PAGE_COUNT}", pageSlot.ToString());
                }
                Console.WriteLine("");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Saving {Groups.ElementAt(i).Key}Shop.yml");
                File.WriteAllText(shopsFolder + $@"\{Groups.ElementAt(i).Key}Shop.yml", Groups.ElementAt(i).Value, Encoding.UTF8);
            }

            
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("########### All Shops has been created ###########");
            Console.WriteLine("Press Enter to exit and show Shop Files...");
            Console.ReadKey();
            Process.Start(shopsFolder);
        }
    }
}
