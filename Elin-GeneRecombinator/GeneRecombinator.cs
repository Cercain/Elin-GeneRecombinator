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
                    p.TrySetAct("Recombinate", () => Recombinate(geneMachine, EClass.pc), __instance.owner, null, 1, false, true, false);
                }
            }
        }

        [HarmonyPrefix,
        HarmonyPatch(typeof(TraitGeneMachine), nameof(TraitGeneMachine.OnUse), new Type[] {typeof(Chara)})]
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

        #region debug overrides

        #endregion
    }
}
