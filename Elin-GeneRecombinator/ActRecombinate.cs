using System;
using System.Collections.Generic;
using System.Text;

namespace Elin_GeneRecombinator
{
    public class ActRecombinate : AIAct
    {
        public override bool PushChara
        {
            get
            {
                return false;
            }
        }

        public override IEnumerable<AIAct.Status> Run()
        {
            if (EClass._zone.IsUserZone)
            {
                Msg.SayNothingHappen();
                yield return this.Cancel();
            }
            yield break;
        }
    }
}
