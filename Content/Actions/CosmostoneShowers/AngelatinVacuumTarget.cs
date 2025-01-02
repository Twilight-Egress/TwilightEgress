using Microsoft.Xna.Framework;
using Terraria;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions.CosmostoneShowers
{
    public class AngelatinVacuumTarget : Node
    {
        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            if (npc.ModNPC is not Angelatin angelatin)
                return NodeState.Failure;

            Player player = Main.player[npc.target];
            Vector2 toNPC = player.DirectionTo(npc.Center) * 0.3f;
            player.velocity += toNPC;

            return base.Update(whoAmI);
        }
    }
}
