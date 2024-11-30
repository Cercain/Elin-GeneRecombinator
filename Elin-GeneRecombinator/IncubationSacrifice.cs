using Empyrean.ColorPicker;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Elin_GeneRecombinator
{
    public class IncubationSacrifice : Chara
    {
        public static IncubationSacrifice _Create(string id, int idMat = -1, int lv = -1)
        {
            IncubationSacrifice chara = new IncubationSacrifice();
            if (lv < 1)
            {
                lv = 1;
            }

            chara.Create(id, idMat, lv);
            if (EClass.player != null)
            {
                EClass.player.codex.AddSpawn(id);
            }

            return chara;
        }

        public override void Die(Element e = null, Card origin = null, AttackSource attackSource = AttackSource.None)
        {
            Console.WriteLine(JsonConvert.SerializeObject(this.held));

            var cons = GetCondition<ConSuspend>();

            if (cons == null) 
            { 
                base.Die(e, origin, attackSource);
                return; 
            }

            foreach (var item in things.ToList())
            {
                if(item.c_DNA != null)
                {
                    if(cons.gene != item)
                    {
                        Card card = EClass._zone.AddCard(item, this.pos);
                        card.OnLand();
                        if (card.trait.CanOnlyCarry)
                        {
                            card.SetPlaceState(PlaceState.installed, false);
                        }
                    }
                }
            }
            this.Destroy();
        }
    }
}
