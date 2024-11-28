﻿using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using System.Linq;
using System.Net.NetworkInformation;
using static UnityEngine.GraphicsBuffer;

namespace Elin_GeneRecombinator
{
    public class InvOwnerGeneRecomb : InvOwnerGene
    {
        public override int numDragGrid => 2;
        public override bool AllowStockIngredients => true;

        public TraitGeneMachine geneMachine;

        public InvOwnerGeneRecomb(TraitGeneMachine gm, Chara _tg = null) : base(gm.owner, _tg) { geneMachine = gm; }

        public override bool ShouldShowGuide(Thing t)
        {
            if(base.dragGrid?.buttons[0]?.Card?.Thing != null)
            {
                var card = base.dragGrid.buttons[0].Card;
                if (card.Thing == t)
                    return false;

                return t.c_DNA != null && card.c_DNA.type == t.c_DNA.type;
            }

            return t.c_DNA != null;
        }

        public override void _OnProcess(Thing t)
        { 
            //Console.WriteLine(JsonConvert.SerializeObject(t));
            TryStartRecombination();
        }

        public void TryStartRecombination()
        {
            var genes = new List<Card>();
            for (int i = 0; i < this.numDragGrid; i++)
            {
                if (base.dragGrid.buttons[i].Card == null)
                {
                    return;
                }
                genes.Add(base.dragGrid.buttons[i].Card);
            }

            SE.Play("mutation");
            this.tg.PlayEffect("identify", true, 0f, default(Vector3));

            var t = genes.First().Thing;

            var newThing = CreateRecombinatedDNA(genes);

            newThing.ChangeMaterial(newThing.c_DNA.GetMaterialId(newThing.c_DNA.type));
            newThing.elements.SetTo(10, 0);           

            var duration = GetRecombDurationFromCost(genes.Select(x => x.c_DNA).ToList());

            var c = CharaGen.Create("gene");

            c.c_altName = "Incubation Sacrifice";
            EClass._zone.AddCard(c, EClass.pc.pos);
            c.Teleport(geneMachine.owner.pos, false, true);
            c.isStolen = true;
            //c.isHidden = true;
            //c.SetHidden(true);
            c.RemoveCondition<ConSleep>();
            c.PlaySound("ride", 1f, true);
            genes.ForEach(x => x.Destroy());
            c.AddCard(newThing);

            var condition = (c.AddCondition<ConSuspend>(100, true) as ConSuspend);
            condition.uidMachine = this.owner.uid;
            condition.gene = newThing;
            condition.duration = duration;
            condition.dateFinish = EClass.world.date.GetRaw(condition.duration);
        }

        public int GetRecombDurationFromCost(List<DNA> genes)
        {
            return (int)(genes.Select(x => x.cost).Aggregate((a, b) => a * b) / 2);
        }

        #region other duration options 
        public int GetRecombDurationFromDurationMulti(List<DNA> genes)
        {
            return (int)(genes.Select(x => x.GetDurationHour()).Aggregate((a, b) => a * b) * .1);
        }
        public int GetRecombDurationFromDuration(List<DNA> genes)
        {
            return (int)(genes.Select(x => x.GetDurationHour()).Aggregate((a, b) => a + b) * .7);
        }
        #endregion

        public Thing CreateRecombinatedDNA(List<Card> cards)
        {
            var first = cards.First();
            var thing = ThingGen.Create("gene", -1, -1);
            //thing.MakeRefFrom(first);
            MakeRefFromGenes(thing, cards);
            DNA dna = new DNA();
            dna.id = "recombinated";
            dna.type = first.c_DNA.type;
            thing.c_DNA = dna;

            var genes = cards.Select(x => x.Thing.c_DNA);
            
            var avg = (int)(genes.Average(x => x.vals.Count)/2);

            var count = UnityEngine.Random.Range(avg-1, avg+2);

            var memories = GenesToMemory(genes);

            var selected = new List<_memory>();
            for (int i = 0; i < count; i++)
            {
                var index = UnityEngine.Random.Range(0, memories.Count);
                selected.Add(memories[index]);
                memories.RemoveAt(index);
            }

            foreach (var item in selected.OrderBy(x => x.id))
            {
                AddVal(dna, item);
            }

            dna.CalcCost();
            return thing;
        }

        private void AddVal(DNA gene, _memory memory)
        {
            int id = memory.id;
            int v = memory.val;
            bool allowStack = true;
            Func<int, int> funcCost = (int v) => v / 5 + 1;

            var ele = Element.Get(id);

            if(ele.category == "feat")
            {
                allowStack = false;
                funcCost = (int v) => ele.cost[0] * 5;
            }
            else if(ele.category == "slot")
            {
                funcCost = (int v) => 20;
            }

            bool flag = false;
            int num = EClass.curve(v, 20, 10, 90);
            v = EClass.curve(v, 20, 10, 80);
            for (int k = 0; k < gene.vals.Count; k += 2)
            {
                if (gene.vals[k] == id)
                {
                    if (allowStack)
                    {
                        if (ele.category == "slot")
                        {
                            gene.vals.Add(id);
                            gene.vals.Add(v);
                            funcCost = (int v) => 40;
                            flag = true;
                        }
                        else
                        {
                            v /= 2;
                            num /= 2;
                            gene.vals[k + 1] += v;
                            Debug.Log(gene.vals[k + 1] + ": " + v + "/" + num);
                            flag = true;
                        }
                        break;
                    }

                    return;
                }
            }

            if (v != 0)
            {
                if (!flag)
                {
                    gene.vals.Add(id);
                    gene.vals.Add(v);
                }

                gene.cost += funcCost(num);
            }
        }

        private List<_memory> GenesToMemory(IEnumerable<DNA> genes)
        {
            var list = new List<_memory>();
            foreach (var gen in genes) 
            {
                for(int i = 0;  i < gen.vals.Count; i+=2)
                {
                    list.Add(new _memory { id = gen.vals[i], val = gen.vals[i+1] });
                }
            }
            return list;
        }

        public void MakeRefFromGenes(Thing c1, List<Card> genes)
        {
            c1.c_idRefCard = "gene";
            c1.c_altName = String.Join(" & ",genes.Select( x=> x.c_altName));
        }

        public struct _memory
        {
            public int id;
            public int val;
        }
    }
}