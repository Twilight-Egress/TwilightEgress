using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.WorldBuilding;
using TwilightEgress.Core.Behavior.BehaviorTrees;
using static Terraria.Utilities.NPCUtils;

namespace TwilightEgress.Content.Actions
{
    public class TargetNPCWithinRange : Node
    {
        private float range;
        private int[] npcsToTarget;

        public TargetNPCWithinRange(float range, params int[] npcsToTarget)
        {
            this.range = range;
            this.npcsToTarget = npcsToTarget;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            bool npcSearchFilter(NPC nPC)
            {
                if (npcsToTarget.Length == 0)
                    return nPC.WithinRange(npc.Center, range);
                else
                    return npcsToTarget.Contains(nPC.type) && nPC.WithinRange(npc.Center, range);
            }

            TargetSearchResults results = SearchForTarget(npc, TargetSearchFlag.NPCs, npcFilter: npcSearchFilter);
            if (results.FoundTarget)
            {
                TargetType targetType = results.NearestTargetType;

                npc.target = results.NearestTargetIndex;
                npc.targetRect = results.NearestTargetHitbox;

                return NodeState.Success;
            }

            return NodeState.Failure;
        }
    }
}
