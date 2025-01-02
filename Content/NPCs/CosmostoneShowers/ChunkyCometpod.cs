using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Actions.CosmostoneShowers.States;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Meteoroids;
using TwilightEgress.Core.Behavior;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class ChunkyCometpod : ModNPC
    {
        public enum CometType
        {
            Meteor,
            Icy,
            ShootingStar
        }

        public enum CometpodBehavior
        {
            PassiveWandering,
            AimlessCharging,
            ChargeTowardsAsteroid,
            ChargeTowardsPlayer,
            Starstruck
        }

        public bool ShouldTargetPlayers;

        public bool ShouldTargetNPCs;

        public bool ShouldStopActivelyTargetting;

        public NPC NearestAsteroid;

        public NPC NearestCometpod;

        public FiniteStateMachine stateMachine;

        public const float MaxPlayerSearchDistance = 300f;

        public const float MaxNPCSearchDistance = 500f;

        public const float MaxTurnAroundCheckDistance = 60f;

        public const float MaxPlayerAggroTimer = 1280f;

        public const float MaxPlayerTargettingChanceReduction = 600f;

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];

        public ref float LocalAIState => ref NPC.ai[2];

        public ref float CurrentCometType => ref NPC.ai[3];

        public ref float PassiveMovementTimer => ref NPC.localAI[0];

        public ref float PassiveMovementSpeed => ref NPC.localAI[1];

        public ref float PassiveMovementVectorX => ref NPC.localAI[2];

        public ref float PassiveMovementVectorY => ref NPC.localAI[3];

        public float PlayerAggroTimer;

        public float ChargeAngle;

        public float MaxAimlessCharges;

        public float AimlessChargeCounter;

        public float MaxStarstruckTime;

        public float PlayerTargettingChanceReduction;

        public float MaxPassiveWanderingTime;

        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.UsesNewTargetting[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            NPC.width = 82;
            NPC.height = 62;
            NPC.damage = 25;
            NPC.defense = 12;
            NPC.knockBackResist = 0.3f;
            NPC.lifeMax = 120;
            NPC.HitSound = SoundID.NPCHit25;
            NPC.DeathSound = SoundID.NPCDeath25;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.value = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Randomly select one of the comet types.
            CurrentCometType = Main.rand.NextFloat(3f);

            stateMachine = new FiniteStateMachine();

            stateMachine.Add((int)CometpodBehavior.PassiveWandering, new PassiveWandering(stateMachine, this));
            stateMachine.Add((int)CometpodBehavior.AimlessCharging, new AimlessCharging(stateMachine, this));
            stateMachine.Add((int)CometpodBehavior.ChargeTowardsAsteroid, new ChargeTowardsAsteroid(stateMachine, this));
            stateMachine.Add((int)CometpodBehavior.ChargeTowardsPlayer, new ChargeTowardsPlayer(stateMachine, this));
            stateMachine.Add((int)CometpodBehavior.Starstruck, new Starstruck(stateMachine, this));

            stateMachine.ForEach((keyValuePair) =>
            {
                keyValuePair.Value.OnExit += Exit;
            });

            // Spawn with either their passive AI or their aimless charging AI.
            stateMachine.SetCurrentState((int)Utils.SelectRandom(Main.rand, CometpodBehavior.PassiveWandering, CometpodBehavior.AimlessCharging), [0f]);

            NPC.scale = Main.rand.NextFloat(0.85f, 1.25f);
            NPC.velocity *= Vector2.UnitX.RotatedByRandom(Math.Tau) * 0.1f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
                writer.Write(NPC.localAI[i]);

            writer.Write(PlayerAggroTimer);
            writer.Write(ChargeAngle);
            writer.Write(MaxAimlessCharges);
            writer.Write(AimlessChargeCounter);
            writer.Write(MaxStarstruckTime);
            writer.Write(PlayerTargettingChanceReduction);
            writer.Write(MaxPassiveWanderingTime);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
                NPC.localAI[i] = reader.ReadSingle();

            PlayerAggroTimer = reader.ReadSingle();
            ChargeAngle = reader.ReadSingle();
            MaxAimlessCharges = reader.ReadSingle();
            AimlessChargeCounter = reader.ReadSingle();
            MaxStarstruckTime = reader.ReadSingle();
            PlayerTargettingChanceReduction = reader.ReadSingle();
            MaxPassiveWanderingTime = reader.ReadSingle();
        }

        public float CalculateCollisionBounceSpeed(float baseSpeed, float velocityDividend = 20f) => baseSpeed * (NPC.velocity.Length() / velocityDividend);

        public void OnHit_HandleExtraVariables()
        {
            if (LifeRatio > 0.5f)
                PlayerAggroTimer = Math.Clamp(PlayerAggroTimer + 180f, 0f, MaxPlayerAggroTimer);
            PlayerTargettingChanceReduction = Math.Clamp(PlayerTargettingChanceReduction + 100f, 0f, MaxPlayerTargettingChanceReduction);
        }

        public override void AI()
        {
            int[] asteroids = [ModContent.NPCType<CosmostoneMeteoroidSmall>(), ModContent.NPCType<CosmostoneMeteoroidMedium>(), ModContent.NPCType<CosmostoneMeteoroidLarge>()];
            if (!ShouldStopActivelyTargetting)
                NPC.AdvancedNPCTargeting(ShouldTargetPlayers, MaxPlayerSearchDistance, ShouldTargetNPCs, MaxNPCSearchDistance, asteroids);
            NPCAimedTarget target = NPC.GetTargetData();

            NearestCometpod = NPC.FindClosestNPC(out _, ModContent.NPCType<ChunkyCometpod>());

            stateMachine?.Update();
            stateMachine?.SetCurrentState((int)AIState);

            // Apply different debuff immunities depending on the style of the Cometpod.
            if (CurrentCometType == (float)CometType.Meteor)
            {
                NPC.Calamity().VulnerableToHeat = false;
                NPC.Calamity().VulnerableToCold = true;
            }

            if (CurrentCometType == (float)CometType.Icy || CurrentCometType == (float)CometType.ShootingStar)
            {
                NPC.Calamity().VulnerableToHeat = true;
                NPC.Calamity().VulnerableToCold = false;
            }

            // Decrement certain values passively.
            PlayerAggroTimer--;
            PlayerTargettingChanceReduction--;

            Timer++;
            NPC.spriteDirection = NPC.direction;
            NPC.AdjustNPCHitboxToScale(82f, 62f);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => OnHit_HandleExtraVariables();

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => OnHit_HandleExtraVariables();

        public void Exit(float[] arguments)
        {
            ShouldStopActivelyTargetting = arguments?[0] == 1f;

            LocalAIState = 0f;
            Timer = 0f;

            PassiveMovementSpeed = 0f;
            PassiveMovementTimer = 0f;
            PassiveMovementVectorX = 0f;
            PassiveMovementVectorY = 0f;

            NPC.netUpdate = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition + Vector2.UnitY;
            Vector2 drawOrigin = texture.Size() * 0.5f;
            SpriteEffects spriteEffects = NPC.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            Main.EntitySpriteDraw(texture, drawPosition, null, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects);
            return false;
        }
    }
}
