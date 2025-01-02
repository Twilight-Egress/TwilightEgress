using Terraria;
using Terraria.WorldBuilding;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions
{
    public class TargetPlayerWithinRange : Node
    {
        private float range;

        public TargetPlayerWithinRange(float range)
        {
            this.range = range;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            npc.TargetClosest();

            if (Main.player[npc.target].WithinRange(npc.Center, range))
                return NodeState.Success;

            return NodeState.Failure;
        }
    }
}
