using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.Actions.CosmostoneShowers
{
    public class AngelatinGasAttack : Node
    {
        private float maxRange;
        private int paralysisTime;
        private int gasExpandTime;

        public AngelatinGasAttack(float maxRange, int paralysisTime, int gasExpandTime)
        {
            this.maxRange = maxRange;
            this.paralysisTime = paralysisTime;
            this.gasExpandTime = gasExpandTime;
        }

        public override NodeState Update(int whoAmI)
        {
            NPC npc = Main.npc[whoAmI];

            if (npc.ModNPC is not Angelatin angelatin)
                return NodeState.Failure;

            npc.velocity *= 0.96f;

            if (IsTargetParalyzed(npc))
                return NodeState.Success;

            angelatin.ResetTimer = false;

            if (angelatin.Timer > 0)
            {
                float radius = MathHelper.Lerp(0, maxRange, angelatin.Timer / gasExpandTime);

                Vector2 targetCenter = npc.HasNPCTarget ? Main.npc[npc.TranslatedTargetIndex].Center : Main.player[npc.target].Center;

                if (targetCenter.WithinRange(npc.Center, radius))
                    AddBuffToTarget(npc);

                Vector2 spawnPosition = npc.Center + Main.rand.NextVector2Circular(radius, radius);
                Color inkColor = Color.SkyBlue;

                for (int i = 0; i <= 4; i++)
                    new AngelatinGasParticle(npc.Center, Main.rand.NextVector2Circular(radius, radius) * 0.05f, inkColor, MathHelper.Lerp(0.2f, 2.5f, angelatin.Timer / gasExpandTime), 0.6f, (int)angelatin.Timer).SpawnCasParticle();

                angelatin.Timer--;
                return NodeState.InProgress;
            }

            return NodeState.Failure;
        }

        private bool IsTargetParalyzed(NPC npc)
        {
            if (npc.HasNPCTarget && Main.npc[npc.TranslatedTargetIndex].GetGlobalNPC<AngelatinNPC>().active)
                return true;
            else if (!npc.HasNPCTarget && Main.player[npc.target].GetModPlayer<AngelatinPlayer>().paralyzed)
                return true;

            return false;
        }

        private void AddBuffToTarget(NPC npc)
        {
            if (npc.HasNPCTarget)
                Main.npc[npc.TranslatedTargetIndex].AddBuff(ModContent.BuffType<AngelatinParalysis>(), paralysisTime);
            else
                Main.player[npc.target].AddBuff(ModContent.BuffType<AngelatinParalysis>(), paralysisTime);
        }
    }
}
