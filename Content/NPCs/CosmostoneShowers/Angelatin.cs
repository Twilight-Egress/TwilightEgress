using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Actions;
using TwilightEgress.Content.Actions.CosmostoneShowers;
using TwilightEgress.Content.Actions.Interfaces;
using TwilightEgress.Core.Behavior.BehaviorTrees;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class Angelatin : ModNPC, IHasHome
    {
        public Vector2 HomePosition { get; set; }

        private static BehaviorTree behaviorTree;

        public ref float Timer => ref NPC.ai[0];

        public bool ResetTimer;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.UsesNewTargetting[Type] = true;

            Selector root = new Selector([
                new Sequence([
                    new Selector([
                        new TargetPlayerWithinRange(320f),
                        new TargetNPCWithinRange(320f)
                    ]),
                    new Selector([
                        new Sequence([
                            new CheckTargetWithinRange(80f),
                            new AngelatinGasAttack(8f * 16f, 4 * 60, 60)
                        ]),
                        new Sequence([
                            new CheckTargetInHomeRange(640f),
                            //new MoveTowardsTarget(3f)
                            new AccelerateTowardsTarget(0.03f, 3.5f)
                        ])
                    ])
                ]),
                new MoveInRangeOfHome(5f, 80f),
                new IdleBobbing()
            ]);

            behaviorTree = new BehaviorTree(root);
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
            ResetTimer = false;
            base.OnSpawn(source);
        }

        public override bool PreAI()
        {
            ResetTimer = true;

            behaviorTree?.Update(NPC.whoAmI);

            if (ResetTimer)
                Timer = 60f;

            if (Main.rand.NextBool(2))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustPosition = NPC.Center + Main.rand.NextVector2CircularEdge(8f * 16f, 8f * 16f);
                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.Electric, Vector2.UnitX);
                    dust.noGravity = true;
                }
            }

            return false;
        }
    }

    public class AngelatinParalysis : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<AngelatinPlayer>().paralyzed = true;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<AngelatinNPC>().active = true;
        }
    }

    public class AngelatinPlayer : ModPlayer
    {
        public bool paralyzed;

        public override void PreUpdateMovement()
        {
            if (paralyzed)
                Player.velocity = Vector2.Zero;
        }

        public override void ResetEffects()
        {
            paralyzed = false;
        }
    }

    public class AngelatinNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;

        public bool active;

        public override bool PreAI(NPC npc)
        {
            if (active)
            {
                npc.velocity = Vector2.Zero;
                //return false;
            }

            return base.PreAI(npc);
        }

        public override void ResetEffects(NPC npc)
        {
            active = false;
        }
    }
}
