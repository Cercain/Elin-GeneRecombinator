using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DG.Tweening;
using HarmonyLib;
using Newtonsoft.Json;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.UI.GridLayoutGroup;

namespace Elin_GeneRecombinator
{
    [HarmonyPatch]
    public class GeneRecombinator
    {
        [HarmonyPostfix, HarmonyPatch(typeof(Trait), nameof(Trait.TrySetAct), new Type[] { typeof(ActPlan) })]
        public static void TrySetAct(Trait __instance, ref ActPlan p)
        {
            if (__instance is TraitGeneMachine geneMachine)
            {
                if (geneMachine.CanUse(geneMachine.owner.Chara) && geneMachine.GetTarget() == null)
                {
                    p.TrySetAct("actRecombinate", () => Recombinate(geneMachine, EClass.pc), __instance.owner, null, 1, false, true, false);
                }
            }
        }

        [HarmonyPrefix,HarmonyPatch(typeof(TraitGeneMachine), nameof(TraitGeneMachine.OnUse), new Type[] {typeof(Chara)})]
        public static bool OnUse(TraitGeneMachine __instance, Chara c)
        {
            if (EClass._zone.IsUserZone)
            {
                Msg.SayNothingHappen();
                return false;
            }
            Chara target = __instance.GetTarget();
            if (target == null) return true;

            if (!target.isStolen) return true;

            if(__instance.GetProgress() >= 1f)
            {
                ConSuspend condition = target.GetCondition<ConSuspend>();
                if (condition.gene.GetRootCard() != target)
                {
                    Msg.SayNothingHappen();
                }
                else
                {
                    target.SetFaction(EClass.game.factions.Wilds);
                    //EClass.pc.Say("gene_finish", target, condition.gene, null, null);
                    EClass.pc.Pick(condition.gene);
                    target.Destroy();
                }
                EClass.pc.PlaySound("ding_potential", 1f, true);
                EClass.pc.pos.PlayEffect("mutation");

                return false;
            }

            return true;
        }

        public static bool Recombinate(TraitGeneMachine gm, Chara c)
        {
            var grid = new InvOwnerGeneRecomb(gm, c);
            LayerDragGrid.Create(grid, false);
            return true;
        }
        
        /*[HarmonyPrefix, HarmonyPatch(typeof(InvOwnerGene), nameof(InvOwnerGene.AllowStockIngredients), MethodType.Getter)]
        public static bool AllowStockIng(InvOwnerGene __instance, ref bool __result)
        {
            Debug.Log("test");
            if (__instance is InvOwnerGene)
                __result = true;
            else __result = __instance.AllowStockIngredients;

            return false;
        }*/

        [HarmonyPrefix,HarmonyPatch(typeof(ConSuspend), nameof(ConSuspend.Tick))]
        public static bool Tick(ConSuspend __instance)
        {
            if (__instance.uidMachine == 0)
            {
                return false;
            }
            TraitGeneMachine traitGeneMachine = __instance.owner.pos.FindThing<TraitGeneMachine>();
            if (traitGeneMachine == null || !traitGeneMachine.owner.isOn || (__instance.duration > 0 && !__instance.HasGene))
            {
                if(__instance.owner.isStolen)
                {
                    Console.WriteLine("[RECOMB] destroying beggar");
                    __instance.owner.SetFaction(EClass.game.factions.Wilds);
                    __instance.owner.Die();
                }
                __instance.Kill(false);
            }
            return false;
        }

        #region debug overrides

        #endregion
    }
}
