using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TwilightEgress.Content.Actions.Interfaces;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions
{
    public class MoveInRangeOfHome : Node
    {
        private float speed;
        private float range;

        public MoveInRangeOfHome(float speed, float range)
        {
            this.speed = speed;
            this.range = range;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            if (npc.ModNPC is not IHasHome home)
                return NodeState.Failure;

            if (npc.Center.WithinRange(home.HomePosition, range))
                return NodeState.Failure;

            npc.velocity = home.HomePosition - npc.Center;
            npc.velocity.Normalize();
            npc.velocity *= speed;

            return NodeState.InProgress;
        }
    }
}
