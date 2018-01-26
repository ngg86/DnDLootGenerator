using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using MyLootGenerator.Classes.ItemClasses;
using MyLootGenerator.Classes.WorkerClasses;

namespace MyLootGenerator.Classes
{
    //Create new cs file for non-roll methods
    //OR
    //non-roll methods move to LootDAL.cs
    internal static class RollMachine
    {
        public static List<int> DieResults;
        public static List<string> GoldDisplay;
        public static string GoldToString;
        public static List<int> GoldResults;
        private static readonly Random _rng = new Random();
        public static List<int> InitialRoll;

        public static List<string> Logger;
        private static string _msg;
        private static bool _addRandomChecked;

        public static List<int> DieRoller(int multiplier, Die? dieType)
        {
            DieResults = new List<int>();

            _msg = "You rolled: " + multiplier + dieType;
            Logger.Add(_msg);

            //Get enum value to string
            var dieSize = dieType.ToString();

            //Remove "d" in enum value so int remains
            dieSize = dieSize.Remove(0, 1);
            var dieNum = int.Parse(dieSize);

            //Roll once per multiplier number and add result to list.
            //Return list when multiplier reaches 0
            _msg = "Your result(s): ";
            Logger.Add(_msg);
            _msg = "";

            for (var i = multiplier; i > 0; i--)
            {
                if (i == 0) return DieResults;

                var num = _rng.Next(1, dieNum);
                DieResults.Add(num);
                _msg += num.ToString() + ", ";
            }

            Logger.Add(_msg);
            return DieResults;
        }

        public static List<string> DetermineLoot(List<object> values, List<string> loot, TextBox logger,
            CheckBox addRandomItem, int numberRandomItems)
        {
            List<string> goldValue;
            const string totalWinnings = "Total Winnings: ";
            Logger = new List<string>();
            Logger.Clear();
            var combatRating = values[0];

            if (values[1].ToString() == "Individual")
            {
                goldValue = RollForGold(combatRating, totalWinnings);
                foreach (var msg in goldValue) loot.Add(msg);
                if (!addRandomItem.Checked) return loot;
                {
                    _addRandomChecked = true;
                    if (numberRandomItems != 0)
                    {
                        var i = 0;
                        while (i < numberRandomItems)
                        {
                            var item = new Loot();
                            var roll = DieRoller(1, Die.d100);

                            //Get numerRandomItems from List<Loot> magicItems @ LootDAL

                            ////GetData() changes this line
                            //var randomItems = GetItemTableValue(combatRating, roll[0]);
                            //if (randomItems.Count >= 1)
                            //{
                            //    var k = _rng.Next(0, randomItems.Count - 1);
                            //    item = randomItems[k];
                            //}

                            //loot.Add("1 x " + item.Name);
                            i++;
                        }
                    }
                    foreach (var msg in Logger)
                    {
                        logger.AppendText(msg);
                        logger.AppendText(Environment.NewLine);
                    }
                }
            }
            else
            {
                goldValue = RollForHoardGold(combatRating, totalWinnings);
                var hoard = RollForTreasure(combatRating);

                foreach (var msg in goldValue) loot.Add(msg);
                if (hoard != null)
                {
                    foreach (var item in hoard) loot.Add(item);
                }
                foreach (var msg in Logger)
                {
                    logger.AppendText(msg);
                    logger.AppendText(Environment.NewLine);
                }
            }
            return loot;
        }

        private static List<string> RollForGold(object cr, string total)
        {
            GoldDisplay = new List<string>();
            GoldResults = new List<int>();
            InitialRoll = new List<int>();
            InitialRoll = DieRoller(1, Die.d100);
            var dieResult = DieResults[0];
            GoldToString = "";
            try
            {
                if (cr.ToString() == CombatRating.Easy.ToString())
                {
                    if (dieResult <= 30)
                    {
                        GoldResults = DieRoller(5, Die.d6);
                        GoldToString = GetGold(GoldResults) + Coins.cp.ToString();
                    }

                    if (dieResult <= 60)
                    {
                        GoldResults = DieRoller(4, Die.d6);
                        GoldToString = GetGold(GoldResults) + Coins.sp.ToString();
                    }

                    if (dieResult <= 70)
                    {
                        GoldResults = DieRoller(1, Die.d6);
                        GoldToString = GetGold(GoldResults) + Coins.gp.ToString();
                    }

                    if (dieResult <= 95)
                    {
                        GoldResults = DieRoller(3, Die.d6);
                        GoldToString = GetGold(GoldResults) + Coins.gp.ToString();
                    }
                    else
                    {
                        GoldResults = DieRoller(1, Die.d6);
                        GoldToString = GetGold(GoldResults) + Coins.pp.ToString();
                    }

                    GoldDisplay.Add(GoldToString);
                    return GoldDisplay;
                }

                if (cr.ToString() == CombatRating.Normal.ToString())
                {
                    if (dieResult <= 30)
                    {
                        GoldResults = DieRoller(4, Die.d6);
                        var gold = GetGold(GoldResults) * 100;

                        GoldResults = DieRoller(1, Die.d6);
                        var secGold = GetGold(GoldResults) * 5;

                        total += (gold / 1000 + secGold).ToString();
                        GoldToString = secGold + Coins.gp.ToString()
                                                + " "
                                                + gold + Coins.cp;
                    }
                    else if (dieResult <= 60)
                    {
                        GoldResults = DieRoller(6, Die.d6);
                        var gold = GetGold(GoldResults) * 10;

                        GoldResults = DieRoller(2, Die.d6);
                        var secGold = GetGold(GoldResults) * 10;

                        total += (gold / 100 + secGold).ToString();
                        GoldToString = secGold + Coins.gp.ToString()
                                                + " "
                                                + gold + Coins.sp;
                    }
                    else if (dieResult <= 70)
                    {
                        GoldResults = DieRoller(3, Die.d6);
                        var gold = GetGold(GoldResults) * 5;

                        GoldResults = DieRoller(2, Die.d6);
                        var secGold = GetGold(GoldResults) * 10;


                        gold = gold + secGold;
                        total += gold.ToString();
                        GoldToString = gold + Coins.gp.ToString();
                    }
                    else if (dieResult <= 95)
                    {
                        GoldResults = DieRoller(4, Die.d6);
                        var gold = GetGold(GoldResults) * 10;

                        total += gold.ToString();
                        GoldToString = gold + Coins.gp.ToString();
                    }
                    else
                    {
                        GoldResults = DieRoller(2, Die.d6);
                        var gold = GetGold(GoldResults) * 10;

                        GoldResults = DieRoller(3, Die.d6);
                        var secGold = GetGold(GoldResults) * 10;

                        total += (secGold * 100 + gold).ToString();
                        GoldToString = secGold + Coins.pp.ToString()
                                                + " "
                                                + gold + Coins.gp;
                    }
                }
                else if (cr.ToString() == CombatRating.Hard.ToString())
                {
                    if (dieResult <= 20)
                    {
                        GoldResults = DieRoller(4, Die.d6);
                        var secGold = GetGold(GoldResults) * 100;

                        GoldResults = DieRoller(1, Die.d6);
                        var gold = GetGold(GoldResults) * 100;

                        total += (secGold / 100 + gold).ToString();

                        GoldToString = gold + Coins.gp.ToString()
                                             + " "
                                             + secGold + Coins.sp;
                    }
                    else if (dieResult <= 35)
                    {
                        GoldResults = DieRoller(1, Die.d6);
                        var gold = GetGold(GoldResults) * 150;

                        total += gold;
                        GoldToString = gold + Coins.gp.ToString();
                    }
                    else if (dieResult <= 75)
                    {
                        GoldResults = DieRoller(2, Die.d6);
                        var secGold = GetGold(GoldResults) * 100;

                        GoldResults = DieRoller(1, Die.d6);
                        var gold = GetGold(GoldResults) * 10;

                        total += (gold * 100 + secGold).ToString();
                        GoldToString = gold + Coins.pp.ToString()
                                             + " "
                                             + secGold + Coins.gp;
                    }
                    else
                    {
                        GoldResults = DieRoller(2, Die.d6);
                        var secGold = GetGold(GoldResults) * 100;

                        GoldResults = DieRoller(2, Die.d6);
                        var gold = GetGold(GoldResults) * 10;

                        total += (gold * 100 + secGold).ToString();
                        GoldToString = gold + Coins.pp.ToString()
                                             + " "
                                             + secGold + Coins.gp;
                    }
                }
                else
                {
                    if (dieResult <= 15)
                    {
                        GoldResults = DieRoller(2, Die.d6);
                        var gold = GetGold(GoldResults) * 500;
                        GoldResults = DieRoller(8, Die.d6);
                        gold = gold + GetGold(GoldResults) * 100;

                        total += gold.ToString();
                        GoldToString = gold + Coins.gp.ToString();
                    }
                    else if (dieResult <= 55)
                    {
                        GoldResults = DieRoller(1, Die.d6);
                        var secGold = GetGold(GoldResults) * 1000;

                        GoldResults = DieRoller(1, Die.d6);
                        var gold = GetGold(GoldResults) * 100;

                        total += (gold * 100 + secGold).ToString();
                        GoldToString = gold + Coins.pp.ToString()
                                             + " "
                                             + secGold + Coins.gp;
                    }
                    else
                    {
                        GoldResults = DieRoller(1, Die.d6);
                        var secGold = GetGold(GoldResults) * 1000;

                        GoldResults = DieRoller(2, Die.d6);
                        var gold = GetGold(GoldResults) * 10;

                        total += (gold * 100 + secGold).ToString();
                        GoldToString = gold + Coins.pp.ToString()
                                             + " "
                                             + secGold + Coins.gp;
                    }
                }

                total += Coins.gp.ToString();
                GoldDisplay.Add(GoldToString);
                GoldDisplay.Add(total);
                return GoldDisplay;
            }
            catch (Exception ex)
            {
                Logger.Add(ex.ToString());
                return null;
            }
        }

        private static List<string> RollForHoardGold(object cr, string total)
        {
            GoldResults = new List<int>();
            GoldDisplay = new List<string>();
            GoldToString = "";
            try
            {
                var gold = 0;
                var secGold = 0;
                var terGold = 0;
                if (cr.ToString() == CombatRating.Easy.ToString())
                {
                    GoldResults = DieRoller(6, Die.d6);
                    terGold = GetGold(GoldResults) * 1000;

                    GoldResults = DieRoller(3, Die.d6);
                    secGold = GetGold(GoldResults) * 100;

                    GoldResults = DieRoller(2, Die.d6);
                    gold = GetGold(GoldResults) * 10;

                    total += (terGold / 1000 + secGold / 100 + gold).ToString();
                    GoldToString = gold + Coins.gp.ToString()
                                         + " "
                                         + secGold + Coins.sp
                                         + " "
                                         + terGold + Coins.cp;
                }
                else if (cr.ToString() == CombatRating.Normal.ToString())
                {
                    GoldResults = DieRoller(2, Die.d6);
                    terGold = GetGold(GoldResults) * 1000;

                    GoldResults = DieRoller(2, Die.d6);
                    secGold = GetGold(GoldResults) * 1000;

                    GoldResults = DieRoller(6, Die.d6);
                    gold = GetGold(GoldResults) * 100;

                    GoldResults = DieRoller(3, Die.d6);
                    var quaGold = GetGold(GoldResults) * 10;

                    total += (quaGold * 100 + terGold / 1000 + secGold / 100 + gold).ToString();
                    GoldToString = quaGold + Coins.pp.ToString()
                                            + " "
                                            + gold + Coins.gp
                                            + " "
                                            + secGold + Coins.sp
                                            + " "
                                            + terGold + Coins.cp;
                }
                else if (cr.ToString() == CombatRating.Hard.ToString())
                {
                    GoldResults = DieRoller(4, Die.d6);
                    secGold = GetGold(GoldResults) * 1000;

                    GoldResults = DieRoller(5, Die.d6);
                    gold = GetGold(GoldResults) * 100;

                    total += (gold * 100 + secGold).ToString();
                    GoldToString = gold + Coins.pp.ToString()
                                         + " "
                                         + secGold + Coins.gp;
                }
                else
                {
                    GoldResults = DieRoller(12, Die.d6);
                    secGold = GetGold(GoldResults) * 1000;

                    GoldResults = DieRoller(8, Die.d6);
                    gold = GetGold(GoldResults) * 1000;

                    total += (gold * 100 + secGold).ToString();
                    GoldToString = gold + Coins.pp.ToString()
                                         + " "
                                         + secGold + Coins.gp;
                }

                total += Coins.gp.ToString();
                GoldDisplay.Add(GoldToString);
                GoldDisplay.Add(total);
                return GoldDisplay;
            }
            catch (Exception ex)
            {
                Logger.Add(ex.ToString());
                return null;
            }
        }

        private static List<string> RollForTreasure(object cr)
        {
            List<string> allLoot = new List<string>();
            InitialRoll = new List<int>();
            InitialRoll = DieRoller(1, Die.d100);
            var result = InitialRoll[0];
            var itemTable = GetItemTableValue(cr, result);
            if (itemTable == null) return null;

            foreach (var item in itemTable)

            {
                var loot = item.Name;
                allLoot.Add(loot);
            }
            var lootList = CheckAndCombineMultiples(allLoot);
            return lootList;
        }

        //NOT FINISHED
        private static List<Loot> GetItemTableValue(object cr, int result)
        {
            List<Loot> magicItems = new List<Loot>();
            List<Loot> artList = new List<Loot>();
            List<Loot> gemList = new List<Loot>();
            List<Loot> secondaryList = new List<Loot>();
            _msg = "Rolling for loot table based on difficulty!";
            Logger.Add(_msg);
            if (cr.ToString() == CombatRating.Easy.ToString())
            {
                if (result <= 6)
                {
                    result = _addRandomChecked ? 45 : result;
                }

                if (result <= 6) return null;
                if (result <= 16)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Common, 2);
                }
                else if (result <= 26)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                }
                else if (result <= 36)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Uncommon, 2);
                }
                else if (result <= 44)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                else if (result <= 52)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                else if (result <= 60)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Rare, 2);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                else if (result <= 65)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(1, Die.d4);
                }
                else if (result <= 70)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(1, Die.d4);
                }
                else if (result <= 75)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Uncommon, 2);
                    magicItems = LootDAL.GetMagicItems(1, Die.d4);
                }
                else if (result <= 78)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(2, Die.d4);
                }
                else if (result <= 80)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(2, Die.d4);

                }
                else if (result <= 85)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Uncommon, 2);
                    magicItems = LootDAL.GetMagicItems(2, Die.d4);
                }
                else if (result <= 92)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(5, Die.d4);
                }
                else if (result <= 97)
                    magicItems = LootDAL.GetMagicItems(5, Die.d4);
                else if (result <= 99)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(6, null);
                }
                else
                {
                    gemList = LootDAL.GetGems(RarityLevel.Uncommon, 2);
                    magicItems = LootDAL.GetMagicItems(6, null);
                }
            }
            else if (cr.ToString() == CombatRating.Normal.ToString())
            {
                if (result <= 4) result = _addRandomChecked ? 29 : result;
                if (result <= 10)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                }
                else if (result <= 16)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Uncommon, 3);
                }
                else if (result <= 22)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Rare, 3);
                }
                else if (result <= 28)
                {
                    artList = LootDAL.GetArts(RarityLevel.Uncommon, 2);
                }
                else if (result <= 32)
                {
                    artList = LootDAL.GetArts(RarityLevel.Common, 2);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                else if (result <= 36)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Uncommon, 3);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                else if (result <= 40)
                {
                    gemList = LootDAL.GetGems(RarityLevel.Rare, 3);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                else if (result <= 44)
                {
                    artList = LootDAL.GetArts(RarityLevel.Uncommon, 2);
                    magicItems = LootDAL.GetMagicItems(0, Die.d6);
                }
                //
                //
                //Continue from here (DMG pg 137)
                //
                //
                else if (result <= 63)
                    magicItems = LootDAL.GetMagicItems(1, Die.d4);
                else if (result <= 74)
                    magicItems = LootDAL.GetMagicItems(2, Die.d4);
                else if (result <= 80)
                    magicItems = LootDAL.GetMagicItems(3, null);
                else if (result <= 94)
                    magicItems = LootDAL.GetMagicItems(5, Die.d4);
                else if (result <= 98)
                    magicItems = LootDAL.GetMagicItems(6, Die.d4);
                else
                    magicItems = LootDAL.GetMagicItems(7, null);
            }
            else if (cr.ToString() == CombatRating.Hard.ToString())
            {
                if (result <= 15) result = _addRandomChecked ? 16 : result;
                if (result <= 29)
                {
                    magicItems = LootDAL.GetMagicItems(0, Die.d4);
                    secondaryList = LootDAL.GetMagicItems(1, Die.d6);
                }
                else if (result <= 50)
                {
                    magicItems = LootDAL.GetMagicItems(2, Die.d6);
                }
                else if (result <= 66)
                {
                    magicItems = LootDAL.GetMagicItems(3, Die.d4);
                }
                else if (result <= 74)
                {
                    magicItems = LootDAL.GetMagicItems(4, null);
                }
                else if (result <= 82)
                {
                    magicItems = LootDAL.GetMagicItems(5, null);
                    secondaryList = LootDAL.GetMagicItems(6, Die.d4);
                }
                else if (result <= 92)
                {
                    magicItems = LootDAL.GetMagicItems(7, Die.d4);
                }
                else
                {
                    magicItems = LootDAL.GetMagicItems(8, null);
                }
            }
            else
            {
                if (result <= 2) result = _addRandomChecked ? 3 : result;
                if (result <= 14)
                    magicItems = LootDAL.GetMagicItems(2, Die.d8);
                else if (result <= 46)
                    magicItems = LootDAL.GetMagicItems(3, Die.d6);
                else if (result <= 68)
                    magicItems = LootDAL.GetMagicItems(4, Die.d6);
                else if (result <= 72)
                    magicItems = LootDAL.GetMagicItems(6, Die.d4);
                else if (result <= 80)
                    magicItems = LootDAL.GetMagicItems(7, Die.d4);
                else
                    magicItems = LootDAL.GetMagicItems(8, Die.d4);
            }
            var lootList = new List<Loot>();

            lootList = MergeLoot(gemList, artList, magicItems, secondaryList);
            return lootList;
            //foreach (var item in itemList) magicItems.Add(item);
            //if (secondaryList.Count < 1) return magicItems;
            //{
            //    foreach (var item in secondaryList)
            //        magicItems.Add(item);
            //}
            //return magicItems;
        }

        //MOVE TO LOOTDAL
        private static List<Loot> MergeLoot(List<Loot> gemList, List<Loot> artList, List<Loot> magicItems, List<Loot> secondaryList)
        {
            List<Loot> allLoot = new List<Loot>();

            if (gemList != null && gemList.Count > 0)
                foreach (var item in gemList)
                {
                    allLoot.Add(item);
                }

            if (artList != null && artList.Count > 0)
                foreach (var item in artList)
                {
                    allLoot.Add(item);
                }

            if (magicItems != null && magicItems.Count > 0)
                foreach (var item in magicItems)
                {
                    allLoot.Add(item);
                }

            if (secondaryList != null && secondaryList.Count > 0)
                foreach (var item in secondaryList)
                {
                    allLoot.Add(item);
                }

            var sortedLoot = allLoot.OrderBy(i => i.Name).ToList();

            return sortedLoot;
        }

        public static List<Loot> RollOnLootPercentage(List<KeyValuePair<Loot, double>> itemsFromTable, Die? dieType)
        {
            List<int> results;
            var itemsFromRoll = new List<Loot>();

            _msg = "Rolling for loot!";
            Logger.Add(_msg);

            if (dieType != null)
            {
                var result = DieRoller(1, dieType);
                results = DieRoller(result[0], Die.d100);
                results.Sort();
            }
            else
            {
                results = DieRoller(1, Die.d100);
            }

            foreach (var dieResult in results)
            {
                var item = GetRolledItems(dieResult, itemsFromTable);
                itemsFromRoll.Add(item);
            }
            return itemsFromRoll;
        }

        private static Loot GetRolledItems(int dieResult, List<KeyValuePair<Loot, double>> itemsFromTable)
        {
            double cumulative = 0;
            foreach (var t in itemsFromTable)
            {
                cumulative += t.Value;
                if (!(dieResult < cumulative)) continue;
                var magicItem = t.Key;
                Loot lootedSpell;
                if (string.Equals(magicItem.Name, "Spell Scroll", StringComparison.CurrentCultureIgnoreCase))
                {
                    var secondaryLoot = GetRolledSpell((MagicItem)magicItem);
                    lootedSpell = new MagicItem();
                    lootedSpell.Name += magicItem.Name + " - \"" + secondaryLoot + "\"";
                }
                else
                {
                    lootedSpell = magicItem;
                }
                return lootedSpell;
            }
            return null;
        }

        private static Loot GetRolledArt(int dieResult, List<KeyValuePair<Loot, double>> artFromTable)
        {
            double cumulative = 0;
            foreach (var a in artFromTable)
            {
                cumulative += a.Value;
                if (!(dieResult < cumulative)) continue;
                return a.Key;
            }
            return null;
        }

        private static Loot GetRolledGems(int dieResult, List<KeyValuePair<Loot, double>> gemFromTable)
        {
            double cumulative = 0;
            foreach (var g in gemFromTable)
            {
                cumulative += g.Value;
                if (!(dieResult < cumulative)) continue;
                return g.Key;
            }
            return null;
        }

        private static string GetRolledSpell(MagicItem spellscroll)
        {

            var result = DieRoller(1, Die.d100);
            var spells = LootDAL.GetSpell(spellscroll, result[0]);
            var spell = spells[0];
            return spell.Name;
        }

        ////make obsolete with GetData();
        //private static List<MagicItem> GetSpell(int secondItemTable, Die? secondDieType, int result)
        //{
        //    _msg = "Getting spells from table!";
        //    Logger.Add(_msg);
        //    //List<MagicItem> itemsFromTable = new List<MagicItem>();
        //    var elements = new List<KeyValuePair<MagicItem, double>>();
        //    const string conString = @"Data Source=DESKTOP-P6OE8JT\SQLEXPRESS;Initial Catalog=MyLootGenerator.Context;Integrated Security=True";
        //    using (var con = new SqlConnection(conString))
        //    {
        //        using (var cmd = new SqlCommand("SELECT Id, Name, Chance FROM MagicItems WHERE ItemTable = @itemTable",
        //            con))
        //        {
        //            cmd.Parameters.AddWithValue("@itemTable", (int)secondItemTable);
        //            con.Open();

        //            using (var reader = cmd.ExecuteReader())
        //            {
        //                while (reader.Read())
        //                {
        //                    var mItem = new MagicItem();

        //                    if (!reader.IsDBNull(0)) mItem.Id = reader.GetInt32(0);
        //                    if (!reader.IsDBNull(1)) mItem.Name = reader.GetString(1);
        //                    if (!reader.IsDBNull(2)) mItem.Chance = (float)reader.GetDouble(2);
        //                    var item = new KeyValuePair<MagicItem, double>(mItem, mItem.Chance);
        //                    elements.Add(item);
        //                }
        //            }
        //        }
        //        var itemsLooted = RollOnLootPercentage(elements, null);
        //        return itemsLooted;
        //    }
        //}


        ////make obsolete with GetData();
        //private static List<Art> GetArt(int result, object cr)
        //{
        //    throw new NotImplementedException();
        //}

        ////make obsolete with GetData();
        //private static List<Gemstone> GetGems(int result, object cr)
        //{
        //    throw new NotImplementedException();
        //}

        //make obsolete with GetData();
        //private static List<string> GetLoot(List<Loot> gemStones, List<Loot> arts, List<Loot> magicItems)
        //{
        //    var loot = new List<string>();
        //    var allLootTypes = new List<ICollection>
        //    {
        //        gemStones,
        //        arts,
        //        magicItems
        //    };
        //    GetLootFromTables(allLootTypes, loot);

        //    return loot;
        //}

        private static List<string> CheckAndCombineMultiples(List<string> allLootTypes)
        {
            List<string> loot = new List<string>();
            var numberItems = 1;
            var prevItem = "";
            var item = "";
            _msg = "Sorting through the loot!";
            Logger.Add(_msg);
            foreach (var lootType in allLootTypes)
            {
                item = "";
                if (lootType == prevItem)
                {
                    numberItems++;
                }
                else
                {
                    if (string.IsNullOrEmpty(prevItem))
                    {
                        prevItem = lootType;
                        item = numberItems + " x " + prevItem;
                    }
                    else
                    {
                        item = numberItems + " x " + prevItem;
                        loot.Add(item);
                        numberItems = 1;
                    }

                    prevItem = lootType;
                }
                item = numberItems + " x " + prevItem;
                //loot.Add(item);
            }
            //adds last item
            loot.Add(item);
            return loot;
        }

        private static int GetGold(List<int> goldResults)
        {
            var gold = 0;
            foreach (var result in goldResults) gold += result;
            return gold;
        }
    }

    //move to own cs file?
    public class Logger
    {
        public List<string> Messages { get; set; }
    }
}