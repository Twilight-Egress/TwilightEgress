using CalamityMod.DataStructures;
using CalamityMod.Items.Weapons.Melee;
using TwilightEgress.Content.NPCs.CosmostoneShowers.Asteroids;
using System.Data.Odbc;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using TwilightEgress.Core.Globals.GlobalNPCs;
using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers.Copepods
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

        public const float MaxPlayerSearchDistance = 300f;

        public const float MaxNPCSearchDistance = 500f;

        public const float MaxTurnAroundCheckDistance = 60f;

        public const float MaxPlayerAggroTimer = 1280f;

        public const float MaxPlayerTargettingChanceReduction = 600f;

        public const int PlayerAggroTimerIndex = 0;

        public const int ChargeAngleIndex = 1;

        public const int MaxAimlessChargesIndex = 2;

        public const int AimlessChargeCounterIndex = 3;

        public const int InitializationIndex = 4;

        public const int MaxStarstruckTimeIndex = 5;

        public const int PlayerTargettingChanceReductionIndex = 6;

        public const int MaxPassiveWanderingTimeIndex = 7;

        public ref float Timer => ref NPC.ai[0];

        public ref float AIState => ref NPC.ai[1];  

        public ref float LocalAIState => ref NPC.ai[2];

        public ref float CurrentCometType => ref NPC.ai[3];

        public ref float PassiveMovementTimer => ref NPC.localAI[0];

        public ref float PassiveMovementSpeed => ref NPC.localAI[1];

        public ref float PassiveMovementVectorX => ref NPC.localAI[2];

        public ref float PassiveMovementVectorY => ref NPC.localAI[3];

        public float LifeRatio => NPC.life / (float)NPC.lifeMax;

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

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

            // Spawn with either their passive AI or their aimless charging AI.
            AIType = Main.rand.Next(2);

            NPC.scale = Main.rand.NextFloat(0.85f, 1.25f);
            NPC.velocity *= Vector2.UnitX.RotatedByRandom(Math.Tau) * 0.1f;
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

        public float CalculateCollisionBounceSpeed(float baseSpeed, float velocityDividend = 20f) => baseSpeed * (NPC.velocity.Length() / velocityDividend);

        public void SwitchAIState(CometpodBehavior behaviorToSwitchTo, bool stopActivelyTargetting = true)
        {
            ShouldStopActivelyTargetting = stopActivelyTargetting;
            AIState = (float)behaviorToSwitchTo;
            LocalAIState = 0f;
            Timer = 0f;

            PassiveMovementSpeed = 0f;
            PassiveMovementTimer = 0f;
            PassiveMovementVectorX = 0f;
            PassiveMovementVectorY = 0f;

            NPC.netUpdate = true;
        }

        public void OnHit_HandleExtraVariables()
        {
            ref float playerAggroTimer = ref NPC.TwilightEgress().ExtraAI[PlayerAggroTimerIndex];
            ref float playerTargettingChanceReduction = ref NPC.TwilightEgress().ExtraAI[PlayerTargettingChanceReductionIndex];

            if (LifeRatio > 0.5f)
                playerAggroTimer = Math.Clamp(playerAggroTimer + 180f, 0f, MaxPlayerAggroTimer);
            playerTargettingChanceReduction = Math.Clamp(playerTargettingChanceReduction + 100f, 0f, MaxPlayerTargettingChanceReduction);
        }

        public override void AI()
        {
            int[] asteroids = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];
            if (!ShouldStopActivelyTargetting)
                NPC.AdvancedNPCTargeting(ShouldTargetPlayers, MaxPlayerSearchDistance, ShouldTargetNPCs, MaxNPCSearchDistance, asteroids);
            NPCAimedTarget target = NPC.GetTargetData();

            ref float playerAggroTimer = ref NPC.TwilightEgress().ExtraAI[PlayerAggroTimerIndex];
            ref float chargeAngle = ref NPC.TwilightEgress().ExtraAI[ChargeAngleIndex];
            ref float maxAimlessCharges = ref NPC.TwilightEgress().ExtraAI[MaxAimlessChargesIndex];
            ref float aimlessChargeCounter = ref NPC.TwilightEgress().ExtraAI[AimlessChargeCounterIndex];
            ref float initialization = ref NPC.TwilightEgress().ExtraAI[InitializationIndex];
            ref float maxStarstruckTime = ref NPC.TwilightEgress().ExtraAI[MaxStarstruckTimeIndex];
            ref float playerTargettingChanceReduction = ref NPC.TwilightEgress().ExtraAI[PlayerTargettingChanceReductionIndex];
            ref float maxPassiveWanderingTime = ref NPC.TwilightEgress().ExtraAI[MaxPassiveWanderingTimeIndex];

            CometType currentCometType = (CometType)CurrentCometType;
            NearestCometpod = NPC.FindClosestNPC(out _, ModContent.NPCType<ChunkyCometpod>());

            switch ((CometpodBehavior)AIState)
            {
                case CometpodBehavior.PassiveWandering:
                    DoBehavior_PassiveWandering(target, ref playerAggroTimer, ref playerTargettingChanceReduction, ref maxPassiveWanderingTime);
                    break;

                case CometpodBehavior.AimlessCharging:
                    DoBehavior_AimlessCharging(target, currentCometType, ref chargeAngle, ref maxAimlessCharges, ref aimlessChargeCounter, ref initialization);
                    break;

                case CometpodBehavior.ChargeTowardsAsteroid:
                    DoBehavior_ChargeTowardsAsteroid(target, currentCometType);
                    break;

                case CometpodBehavior.ChargeTowardsPlayer:
                    DoBehavior_ChargeTowardsPlayer(target, currentCometType);
                    break;

                case CometpodBehavior.Starstruck:
                    DoBehavior_Starstruck(target, currentCometType, ref initialization, ref maxStarstruckTime);
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

            // Decrement certain values passively.
            playerAggroTimer--;
            playerTargettingChanceReduction--;

            Timer++;
            NPC.spriteDirection = NPC.direction;
            NPC.AdjustNPCHitboxToScale(82f, 62f);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) => OnHit_HandleExtraVariables();

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) => OnHit_HandleExtraVariables();

        public void DoBehavior_PassiveWandering(NPCAimedTarget target, ref float playerAggroTimer, ref float playerTargettingChanceReduction, ref float maxPassiveWanderingTime)
        {
            // Initialize a few values after each chance to switch AI.
            if (Timer is 0)
            {
                maxPassiveWanderingTime = Main.rand.Next(480, 1200);
                return;
            }

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

            NPC.CheckForTurnAround(-MathHelper.PiOver2, MathHelper.PiOver2, 0.05f, out bool shouldTurnAround);
            Vector2 centerAhead = NPC.Center + NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            // Avoid tiles and leaving space. 
            if (shouldTurnAround || leavingSpace)
            {
                float distanceFromTileCollisionLeft = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(-MathHelper.PiOver2)) ?? 1000f;
                float distanceFromTileCollisionRight = Utilities.DistanceToTileCollisionHit(NPC.Center, NPC.velocity.RotatedBy(MathHelper.PiOver2)) ?? 1000f;
                int directionToMove = distanceFromTileCollisionLeft > distanceFromTileCollisionRight ? -1 : 1;
                Vector2 turnAroundVelocity = NPC.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2 * directionToMove);
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
                float moveSpeed = PassiveMovementSpeed / MathF.Sqrt(MathF.Pow(PassiveMovementVectorX, 2) + MathF.Pow(PassiveMovementVectorY, 2));
                NPC.velocity = Vector2.Lerp(NPC.velocity, new Vector2(PassiveMovementVectorX * moveSpeed, PassiveMovementVectorY * moveSpeed) * 3f, 0.02f);
            }

            ShouldTargetNPCs = true;
            ShouldTargetPlayers = true;

            // Randomly select an asteroid and switch AI states.
            int[] asteroids = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];
            if (Main.rand.NextBool(1500) && ShouldTargetNPCs && target.Type == Terraria.Enums.NPCTargetType.NPC && !target.Invalid)
            {
                NearestAsteroid = NPC.FindClosestNPC(out float _, asteroids);
                SwitchAIState(CometpodBehavior.ChargeTowardsAsteroid);
            }

            // Randomly select a player and switch AI states.
            int playerTargetChance = (int)(1500 - playerTargettingChanceReduction);
            if (Main.rand.NextBool(playerTargetChance) && ShouldTargetPlayers && target.Type == Terraria.Enums.NPCTargetType.Player && !target.Invalid)
                SwitchAIState(CometpodBehavior.ChargeTowardsPlayer);

            if (Timer >= maxPassiveWanderingTime && Main.rand.NextBool(5))
                SwitchAIState(CometpodBehavior.AimlessCharging, false);
            else
            {
                Timer = 0f;
                NPC.netUpdate = true;
            }

            NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() - MathHelper.Pi, 0.2f); 
        }

        public void DoBehavior_AimlessCharging(NPCAimedTarget target, CometType cometType, ref float chargeAngle, ref float maxAimlessCharges, ref float aimlessChargeCounter, ref float initialization)
        {
            int lineUpTime = 75;
            int chargeTime = 240;
            int postBonkCooldownTime = 180;
            int[] asteroids = [ModContent.NPCType<CosmostoneAsteroidSmall>(), ModContent.NPCType<CosmostoneAsteroidMedium>(), ModContent.NPCType<CosmostoneAsteroidLarge>()];

            // Initialize and pick a random amount of times to charge.
            if (initialization == 0f)
            {
                maxAimlessCharges = Main.rand.Next(3, 6);
                aimlessChargeCounter = 0f;
                initialization = 1f;
                NPC.netUpdate = true;
            }

            ShouldTargetPlayers = true;
            ShouldTargetNPCs = true;

            Vector2 centerAhead = NPC.Center + NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            if (LocalAIState == 0f)
            {
                // Pick a random angle to turn towards and charge at.
                if (Timer is 1)
                    chargeAngle = Main.rand.NextFloat(MathF.Tau);

                NPC.rotation = NPC.rotation.AngleLerp(chargeAngle - MathHelper.Pi, 0.2f);
                NPC.velocity *= 0.9f;

                if (Timer >= lineUpTime)
                {
                    Timer = 0f;
                    LocalAIState = 1f;
                    NPC.netUpdate = true;
                }
            }

            if (LocalAIState == 1f)
            {
                Vector2 chargeVelocity = chargeAngle.ToRotationVector2() * 10f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, chargeVelocity, 0.06f);
                NearestAsteroid = NPC.FindClosestNPC(out _, asteroids);

                void switchToCooldown(ref float aimlessChargeCounter)
                {
                    aimlessChargeCounter++;
                    Timer = 0f;
                    LocalAIState = 2f;
                    NPC.netUpdate = true;
                }

                // Simply stop if either the max time is reached or if the cometpod is outside of the space boundaries.
                if (Timer >= chargeTime || leavingSpace)
                    switchToCooldown(ref aimlessChargeCounter);

                // Bump into any tiles.
                if (NPC.collideX || NPC.collideY)
                {
                    NPC.velocity = NPC.oldVelocity * CalculateCollisionBounceSpeed(-0.42f);
                    switchToCooldown(ref aimlessChargeCounter);
                }
                
                // Bump into nearby cometpods if collision between the two occurs.
                if (NearestCometpod is not null && NPC.Hitbox.Intersects(NearestCometpod.Hitbox))
                {
                    NPC.velocity = NPC.DirectionFrom(NearestCometpod.Center) * CalculateCollisionBounceSpeed(0.8f);
                    NearestCometpod.velocity = NearestCometpod.DirectionFrom(NPC.Center) * CalculateCollisionBounceSpeed(1f);

                    // Stun the other cometpod.
                    if (NearestCometpod.ModNPC is ChunkyCometpod cometpod && cometpod.AIState != (float)CometpodBehavior.AimlessCharging)
                        cometpod.SwitchAIState(CometpodBehavior.Starstruck, false);

                    switchToCooldown(ref aimlessChargeCounter);
                }

                // Bump into the player if collision between the two occurs.
                if (!target.Invalid && target.Type == Terraria.Enums.NPCTargetType.Player && NPC.Hitbox.Intersects(target.Hitbox))
                {
                    NPC.velocity = NPC.DirectionFrom(target.Center) * CalculateCollisionBounceSpeed(0.8f);
                    target.Velocity = target.Center.DirectionFrom(NPC.Center) * CalculateCollisionBounceSpeed(2f);
                    switchToCooldown(ref aimlessChargeCounter);
                }

                // Bump into any nearby asteroids if collision between the two occurs.
                if (NearestAsteroid is not null && NPC.Hitbox.Intersects(NearestAsteroid.Hitbox))
                {
                    NPC.velocity = NPC.DirectionFrom(NearestAsteroid.Center) * CalculateCollisionBounceSpeed(0.86f);
                    NearestAsteroid.velocity = NearestAsteroid.DirectionFrom(NPC.Center) * CalculateCollisionBounceSpeed(1f);

                    int damageTaken = (int)(Main.rand.Next(1, 3) * NPC.velocity.Length());
                    NPC.SimpleStrikeNPC(damageTaken, NPC.direction, noPlayerInteraction: true);
                    NearestAsteroid.SimpleStrikeNPC(damageTaken * 8, -NPC.direction, noPlayerInteraction: true);
                    switchToCooldown(ref aimlessChargeCounter);
                }
            }

            if (LocalAIState == 2f)
            {
                NPC.rotation += NPC.velocity.X * 0.03f;
                NPC.velocity *= 0.98f;

                if (Timer >= postBonkCooldownTime)
                {
                    if (aimlessChargeCounter >= maxAimlessCharges || leavingSpace)
                    {
                        SwitchAIState(CometpodBehavior.PassiveWandering, false);
                    }
                    else
                    {
                        Timer = 0f;
                        LocalAIState = 0f;
                        NPC.netUpdate = true;
                    }
                }
            }
        }

        public void DoBehavior_ChargeTowardsAsteroid(NPCAimedTarget target, CometType cometType)
        {
            if (target.Invalid || NearestAsteroid is null)
            {
                SwitchAIState(CometpodBehavior.Starstruck, false);
                return;
            }

            int lineUpTime = 75;
            int chargeTime = 240;

            Vector2 centerAhead = NPC.Center + NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            if (LocalAIState == 0f)
            {
                NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center) - MathHelper.Pi, 0.2f);
                NPC.velocity *= 0.9f;

                if (Timer >= lineUpTime)
                {
                    Timer = 0f;
                    LocalAIState = 1f;
                    NPC.velocity = NPC.SafeDirectionTo(target.Center);
                    NPC.netUpdate = true;
                }
            }

            if (LocalAIState == 1f)
            {
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.Pi;

                if (NPC.velocity.Length() < 10f)
                    NPC.velocity *= 1.06f;                    

                // Bounce off of the target when collision is made.
                if (NPC.Hitbox.Intersects(target.Hitbox))
                {
                    NPC.velocity = NPC.DirectionFrom(target.Center) * CalculateCollisionBounceSpeed(0.86f);
                    target.Velocity = target.Center.DirectionFrom(NPC.Center) * CalculateCollisionBounceSpeed(1f);

                    int damageTaken = (int)(Main.rand.Next(1, 3) * NPC.velocity.Length());
                    NPC.SimpleStrikeNPC(damageTaken, NPC.direction, noPlayerInteraction: true);
                    NearestAsteroid.SimpleStrikeNPC(damageTaken * 8, -NPC.direction, noPlayerInteraction: true);

                    NearestAsteroid = null;
                    SwitchAIState(CometpodBehavior.Starstruck, false);
                }

                // If no collision is made or the Cometpod is out of bounds, simply switch AI states.
                if (leavingSpace || Timer >= chargeTime || NPC.collideX || NPC.collideY)
                {
                    if (NPC.collideX || NPC.collideY)
                        NPC.velocity = NPC.oldVelocity * CalculateCollisionBounceSpeed(-0.42f);

                    SwitchAIState(CometpodBehavior.Starstruck, false);
                }

                // Bounce off of other Cometpods.
                if (NearestCometpod is not null && NPC.Hitbox.Intersects(NearestCometpod.Hitbox))
                {
                    NPC.velocity = NPC.oldVelocity * -0.8f;
                    NearestCometpod.velocity = NearestCometpod.DirectionFrom(NPC.Center) * NPC.velocity.Length();

                    SwitchAIState(CometpodBehavior.Starstruck, false);
                }
            }
        }

        public void DoBehavior_ChargeTowardsPlayer(NPCAimedTarget target, CometType cometType)
        {
            if (target.Invalid)
            {
                SwitchAIState(CometpodBehavior.Starstruck, false);
                return;
            }

            int lineUpTime = 75;
            int chargeTime = 240;

            Vector2 centerAhead = NPC.Center + NPC.velocity * MaxTurnAroundCheckDistance;
            bool leavingSpace = centerAhead.Y >= Main.maxTilesY + 750f || centerAhead.Y < Main.maxTilesY * 0.34f;

            if (LocalAIState == 0f)
            {
                NPC.rotation = NPC.rotation.AngleLerp(NPC.AngleTo(target.Center) - MathHelper.Pi, 0.2f);
                NPC.velocity *= 0.9f;

                if (Timer >= lineUpTime)
                {
                    Timer = 0f;
                    LocalAIState = 1f;
                    NPC.velocity = NPC.SafeDirectionTo(target.Center);
                    NPC.netUpdate = true;
                }
            }

            if (LocalAIState == 1f)
            {
                NPC.rotation = NPC.velocity.ToRotation() - MathHelper.Pi;

                if (NPC.velocity.Length() < 10f)
                    NPC.velocity *= 1.06f;

                // Bounce off of the target when collision is made.
                if (NPC.Hitbox.Intersects(target.Hitbox))
                {
                    NPC.velocity = NPC.DirectionFrom(target.Center) * CalculateCollisionBounceSpeed(0.8f);
                    target.Velocity = target.Center.DirectionFrom(NPC.Center) * CalculateCollisionBounceSpeed(2f);

                    SwitchAIState(CometpodBehavior.Starstruck, false);
                }

                // If no collision is made or the Cometpod is out of bounds, simply switch AI states.
                if (leavingSpace || Timer >= chargeTime || NPC.collideX || NPC.collideY)
                {
                    if (NPC.collideX || NPC.collideY)
                        NPC.velocity = NPC.oldVelocity * CalculateCollisionBounceSpeed(-0.42f);

                    SwitchAIState(CometpodBehavior.Starstruck, false);
                }

                // Bounce off of other Cometpods.
                if (NearestCometpod is not null && NPC.Hitbox.Intersects(NearestCometpod.Hitbox))
                {
                    NPC.velocity = NPC.oldVelocity * -0.8f;
                    NearestCometpod.velocity = NearestCometpod.DirectionFrom(NPC.Center) * NPC.velocity.Length();

                    SwitchAIState(CometpodBehavior.Starstruck, false);
                }
            }
        }

        // @ zarachard
        public void DoBehavior_Starstruck(NPCAimedTarget target, CometType cometType, ref float initialization, ref float maxStarstruckTime)
        {
            if (initialization is 0)
            {
                maxStarstruckTime = Main.rand.Next(180, 300);
                initialization = 1f;
                NPC.netUpdate = true;
            }

            NPC.velocity *= 0.98f;
            NPC.rotation += NPC.velocity.X * 0.03f;

            if (Timer >= maxStarstruckTime)
            {
                maxStarstruckTime = 0f;
                initialization = 0f;
                SwitchAIState(CometpodBehavior.PassiveWandering, false);
            }
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
