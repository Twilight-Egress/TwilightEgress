using Terraria;
using TwilightEgress.Core.Behavior;
using static TwilightEgress.Content.NPCs.CosmostoneShowers.ChunkyCometpod;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior
{
    // @ zarachard
    public class Starstruck(FiniteStateMachine stateMachine, ChunkyCometpod cometpod) : EntityState<ChunkyCometpod>(stateMachine, cometpod)
    {
        public override void Enter(float[] arguments = null)
        {
            Entity.MaxStarstruckTime = Main.rand.Next(180, 300);
            Entity.AIState = (int)CometpodBehavior.Starstruck;
            Entity.NPC.netUpdate = true;
        }

        public override void Update(float[] arguments = null)
        {
            // extremely simple in comparison i know
            Entity.NPC.velocity *= 0.98f;
            Entity.NPC.rotation += Entity.NPC.velocity.X * 0.03f;

            if (Entity.Timer >= Entity.MaxStarstruckTime)
            {
                Entity.MaxStarstruckTime = 0f;
                FiniteStateMachine.SetCurrentState((int)CometpodBehavior.PassiveWandering, [0f]);
            }
        }
    }
}
