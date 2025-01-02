﻿using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions
{
    public class IdleBobbing : Node
    {
        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            npc.velocity *= 0.94f;

            return NodeState.InProgress;
        }
    }
}