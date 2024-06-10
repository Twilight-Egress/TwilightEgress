using CalamityMod.Items.Fishing;
using Cascade.Content.NPCs.CosmostoneShowers.Asteroids;
using System.Runtime.InteropServices;
using Terraria;

namespace Cascade.Content.NPCs.CosmostoneShowers.Copepods
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
            AimlessWandering,
            ChargeTowardsAsteroid,
            ChargeTowardsPlayer
        }

        public bool ShouldTargetPlayers;

        public bool ShouldTargetNPCs;

        public NPC SelectedAsteroid;

        public const float MaxPlayerSearchDistance = 300f;

        public const float MaxNPCSearchDistance = 500f;

        public const float MaxTurnAroundCheckDistance = 60f;

        public const int PlayerAggroTimerIndex = 0;

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];  

        public ref float LocalAIState => ref NPC.ai[2];

        public ref float CurrentCometType => ref NPC.ai[3];

        public ref float PassiveMovementTimer => ref NPC.localAI[0];

        public ref float PassiveMovementSpeed => ref NPC.localAI[1];

        public ref float PassiveMovementVectorX => ref NPC.localAI[2];

        public ref float PassiveMovementVectorY => ref NPC.localAI[3];

        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.UsesNewTargetting[Type] = true;
            NPCID.Sets.TrailCacheLength[Type] = 10;
            NPCID.Sets.TrailingMode[Type] = 3;
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
            NPC.scale = Main.rand.NextFloat(0.85f, 1.25f);
            NPC.velocity *= Vector2.UnitX.RotatedByRandom(Tau) * 0.1f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
                writer.Write(NPC.localAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            for (int i = 0; i < NPC.localAI.Length; i++)
                NPC.localAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            int[] asteroids = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];
            NPC.AdvancedNPCTargeting(ShouldTargetPlayers, MaxPlayerSearchDistance, ShouldTargetNPCs, MaxNPCSearchDistance, asteroids);
            NPCAimedTarget target = NPC.GetTargetData();

            ref float playerAggroTimer = ref NPC.Cascade().ExtraAI[PlayerAggroTimerIndex];
            CometType currentCometType = (CometType)CurrentCometType;

            switch ((CometpodBehavior)AIState)
            {
                case CometpodBehavior.PassiveWandering:
                    DoBehavior_PassiveWandering(target, ref playerAggroTimer);
                    break;

                case CometpodBehavior.ChargeTowardsAsteroid:
                    break;

                case CometpodBehavior.ChargeTowardsPlayer:
                    break;
            }

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

            Timer++;
            NPC.spriteDirection = NPC.direction;
            NPC.AdjustNPCHitboxToScale(82f, 62f);
        }

        public void DoBehavior_PassiveWandering(NPCAimedTarget target, ref float playerAggroTimer)
        {
            // Move slowly in a random direction every few seconds.
            PassiveMovementTimer--;
            if (PassiveMovementTimer <= 0f)
            {
                PassiveMovementSpeed = Main.rand.NextFloat(5f, 151f) * 0.01f; 
                PassiveMovementVectorX = Main.rand.NextFloat(-100f, 101f);
                PassiveMovementVectorY = Main.rand.NextFloat(-100f, 101f);
                PassiveMovementTimer = Main.rand.NextFloat(120f, 360f);
                NPC.netUpdate = true;
            }

            NPC.CheckForTurnAround(-PiOver2, PiOver2, 0.05f, out bool shouldTurnAround);
            Vector2 centerAhead = NPC.Center + NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            // Avoid tiles and leaving space. 
            if (shouldTurnAround || leavingSpace)
            {
                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(PiOver2)) ?? 1000f;
                int directionToMove = distanceFromTileCollisionLeft > distanceFromTileCollisionRight ? -1 : 1;
                Vector2 turnAroundVelocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(PiOver2 * directionToMove);
                if (leavingSpace)
                    turnAroundVelocity = centerAhead.Y >= Main.maxTilesY + 750f ? Vector2.UnitY * -3f : centerAhead.Y < Main.maxTilesY * 0.34f ? Vector2.UnitY * 3f : NPC.velocity;

                // Setting these ensures that once the turnAround check becomes false, the normal idle velocity
                // won't conflict with the turn around velocity.
                PassiveMovementVectorX = turnAroundVelocity.X;
                PassiveMovementVectorY = turnAroundVelocity.Y;

                NPC.velocity = Vector2.Lerp(NPC.velocity, turnAroundVelocity, 0.1f);
            }
            else
            {
                float moveSpeed = PassiveMovementSpeed / Sqrt(Pow(PassiveMovementVectorX, 2) + Pow(PassiveMovementVectorY, 2));
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(PassiveMovementVectorX * moveSpeed, PassiveMovementVectorY * moveSpeed) * 3f, 0.02f);
            }

            if ((LifeRatio <= 0.5f || playerAggroTimer > 0) && !ShouldTargetPlayers)
            {
                ShouldTargetPlayers = true;
                NPC.netUpdate = true;
            }
            else
            {
                ShouldTargetPlayers = false;
                ShouldTargetNPCs = true;
            }

            NPC.rotation = NPC.velocity.ToRotation() - Pi;
        }

        public void DoBehavior_ChargeTowardsAsteroid(NPCAimedTarget target, CometType cometType)
        {

        }

        public void DoBehavior_ChargeTowardsPlayer(NPCAimedTarget target, CometType cometType)
        {

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Vector2 drawPosition = NPC.Center - Main.screenPosition + Vector2.UnitY;
            Vector2 drawOrigin = texture.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, null, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, NPC.DirectionBasedSpriteEffects());
            return false;
        }
    }
}
