using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace Elin_GeneRecombinator
{
    [HarmonyPatch(typeof(SourceManager), nameof(SourceManager.Init))]
    public class SourceFix
    {
        static bool initialized = false;

        [HarmonyPostfix]
        public static void Postfix(SourceManager __instance) 
        {
            if (initialized)
                return;

            LangGeneral.Row actRecombinate = new LangGeneral.Row
            {
                id= "actRecombinate",
                filter="act",
                text="Recombinate",
                text_JP= "遺伝子再集合"
            };

            LangGeneral.Row cardRecombinate = new LangGeneral.Row
            {
                id = "recombinated",
                text = "Recombinated",
                text_JP = "変更された"
            };

            __instance.langGeneral.rows.Add(actRecombinate);
            __instance.langGeneral.SetRow(actRecombinate);
            __instance.langGeneral.rows.Add(cardRecombinate);
            __instance.langGeneral.SetRow(cardRecombinate);
            initialized = true;
        }
    }
}
