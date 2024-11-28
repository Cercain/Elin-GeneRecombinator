using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Serialization;
using BepInEx;
using HarmonyLib;
using Newtonsoft.Json;

namespace Elin_GeneRecombinator
{
    [BepInPlugin("net.cercain.generecombinator", "GeneRecombinator", "0.0.1")]
    public class Mod_GeneRecombinator : BaseUnityPlugin
    {
        private void Awake()
        {
            System.Console.WriteLine("Loading GeneRecombinator");
            new Harmony("GeneRecombinator").PatchAll();
        }
    }
}
