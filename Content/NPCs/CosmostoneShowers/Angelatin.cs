using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Behavior.Interfaces;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class Angelatin : ModNPC, IHasHome
    {
        public Vector2 HomePosition { get; set; }

        private static BehaviorTree behaviorTree;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.UsesNewTargetting[Type] = true;
        }

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
            NPC.noTileCollide = true;

            Selector root = new Selector([
                new Sequence([
                    new Selector([
                        new TargetPlayerWithinRange(320f),
                        new TargetNPCWithinRange(320f)
                    ]),
                    new Sequence([
                        new CheckTargetInHomeRange(640f),
                        new MoveTowardsTarget(3f)
                    ])
                ]),
                new MoveInRangeOfHome(5f, 80f),
                new IdleBobbing()
            ]);

            behaviorTree = new BehaviorTree(root);
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HomePosition.X);
            writer.Write(HomePosition.Y);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Vector2 homePosition = Vector2.Zero;

            homePosition.X = reader.ReadSingle();
            homePosition.Y = reader.ReadSingle();

            HomePosition = homePosition;
        }

        public override void OnSpawn(IEntitySource source)
        {
            HomePosition = NPC.Center;

            NPC.netUpdate = true;
            base.OnSpawn(source);
        }

        public override bool PreAI()
        {
            behaviorTree?.Update(NPC.whoAmI);

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustPosition = HomePosition + Main.rand.NextVector2CircularEdge(640f, 640f);
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Electric, Vector2.UnitX, newColor: Color.Pink);
                    dust.noGravity = true;
                }
            }

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustPosition = HomePosition + Main.rand.NextVector2CircularEdge(80f, 80f);
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Electric, Vector2.UnitX, newColor: Color.Red);
                    dust.noGravity = true;
                }
            }

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustPosition = NPC.Center + Main.rand.NextVector2CircularEdge(320f, 320f);
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Electric, Vector2.UnitX);
                    dust.noGravity = true;
                }
            }

            return false;
        }
    }
}
