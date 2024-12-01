using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;

namespace Elin_GeneRecombinator
{
    [BepInPlugin("cercain.generecombinator", "GeneRecombinator", "1.1.0")]
    public class Mod_GeneRecombinator : BaseUnityPlugin
    {
        private ConfigFile configFile = new ConfigFile(Path.Combine(Paths.ConfigPath, "cercain.GeneticRecombination.cfg"), true);
        public static ConfigEntry<int> MemoryAverageMin { get; set; }
        public static ConfigEntry<int> MemoryAverageMax { get; set; }
        public static ConfigEntry<bool> WeightMemoryByPower { get; set; }
        public static ConfigEntry<int> GeneUpgradeChance { get; set; }

        private void Awake()
        {
            MemoryAverageMin = configFile.Bind<int>("Settings", "MemoryAverageMin", -1, "The amount taken from the average amount of genes of the inputs to give the min range");
            MemoryAverageMax = configFile.Bind<int>("Settings", "MemoryAverageMax", 1, "The amount added to the average amount of genes of the inputs to give the max range");
            WeightMemoryByPower = configFile.Bind<bool>("Settings", "WeightMemoryByPower", true, "Weight the chance of a memory being selected by it's feat cost?");
            GeneUpgradeChance = configFile.Bind<int>("Settings", "GeneUpgradeChance", 10, "1 in X Chance for a normal gene recombination to upgrade to a superior gene. 0 to disable");    

            new Harmony("GeneRecombinator").PatchAll();
            Debug.Log($"Loaded");
        }
    }
}
