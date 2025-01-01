using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.Demo.Behavior;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.Demo
{
    public class BehaviorTreeDemoNPC : ModNPC
    {
        public override string Texture => "TwilightEgress/Assets/Textures/NPCs/CosmostoneShowers/Angelatin";

        private BehaviorTree behaviorTree;

        public override void SetDefaults()
        {
            NPC.width = 32;
            NPC.height = 34;
            NPC.damage = 25;
            NPC.defense = 3;
            NPC.lifeMax = 90;
            NPC.knockBackResist = 0.8f;
            NPC.value = 9f;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath25;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Selector root = new Selector([
                new Sequence([
                    new PlayerWithinRange(20 * 16),
                    new MoveTowardsPlayer(2.5f)
                ]),
                new IdleRotate(0.05f, 0.95f)
            ]);

            behaviorTree = new BehaviorTree(root);
            base.OnSpawn(source);
        }

        public override bool PreAI()
        {
            behaviorTree?.Update(NPC.whoAmI);
            return false;
        }
    }
}
