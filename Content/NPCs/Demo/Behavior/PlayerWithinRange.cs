using Terraria;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.Demo.Behavior
{
    public class PlayerWithinRange : Node
    {
        private float range;

        public PlayerWithinRange(float range)
        {
            this.range = range;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            npc.TargetClosest();

            if (Main.player[npc.target].Center.Distance(npc.Center) <= range)
                return NodeState.Success;

            return NodeState.Failure;
        }
    }
}
