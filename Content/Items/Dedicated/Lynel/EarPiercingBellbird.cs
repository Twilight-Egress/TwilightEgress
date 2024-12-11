using CalamityMod;
using Luminance.Core.Graphics;
using Terraria;
using TwilightEgress.Assets;
using TwilightEgress.Content.Buffs.Debuffs;
using TwilightEgress.Content.Buffs.Pets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Content.Items.Dedicated.Lynel
{
    public class EarPiercingBellbird : ModProjectile, ILocalizedModType
    {
        public enum BellbirdStates
        {
            Flying,
            Perching,
            CryOfGod
        }

        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public ref float AIState => ref Projectile.ai[1];

        public ref float LocalAIState => ref Projectile.ai[2];

        private const int ScreamChanceRegular = 1000000000;

        private const int ScreamChanceGFB = 100000;

        private const int ScreamChargeTime = 120;

        private const int ScreamTime = 360;

        private const int TimeToPerch = 360;

        private const int ScreamChargeVisualScaleIndex = 0;

        private const int ScreamChargeVisualOpacityIndex = 1;

        public new string LocalizationCategory => "Projectiles.Pets";

        public override void SetStaticDefaults()
        {
            Main.projPet[Type] = true;
            Main.projFrames[Type] = 5;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            if (Owner.active && Owner.HasBuff(ModContent.BuffType<BellbirdBuff>()))
                Projectile.timeLeft = 2;

            GameTime gameTime = Main.gameTimeCache;
            int screamChance = Main.zenithWorld ? ScreamChanceGFB : ScreamChanceRegular;
            bool canChirp = AIState != (float)BellbirdStates.CryOfGod && gameTime.TotalGameTime.Ticks % 60 == 0 && Main.rand.NextBool(100);
            bool canScream = AIState != (float)BellbirdStates.CryOfGod && gameTime.TotalGameTime.Ticks % 720 == 0f && Main.rand.NextBool(screamChance);

            switch ((BellbirdStates)AIState)
            {
                case BellbirdStates.Flying:
                    DoBehavior_Flying();
                    break;

                case BellbirdStates.Perching:
                    DoBehavior_Perching();
                    break;

                case BellbirdStates.CryOfGod:
                    DoBehavior_CryOfGod();
                    break;
            }

            // Try to run the bellbird scream at the respective random chance every 12 seconds.
            if (canScream)
            {
                AIState = (float)BellbirdStates.CryOfGod;
                Timer = 0f;
                LocalAIState = 0f;
                Projectile.netUpdate = true;
            }

            // Chirp occasionally.
            if (canChirp)
            {
                SoundEngine.PlaySound(AssetRegistry.Sounds.BellbirdChirp, Projectile.Center);

                Vector2 velocity = new(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4f, -2f));
                new MusicNoteParticle(Projectile.Center, velocity).Spawn();
            }

            // Stop perching if the player inverts their gravity.
            bool orbitalGravity = Owner.GetModPlayer<OrbitalGravityPlayer>().Planetoid is not null && Owner.GetModPlayer<OrbitalGravityPlayer>().Planetoid.NPC.active;
            bool shouldStopPerching = Owner.gravDir == -1 || orbitalGravity;
            if (shouldStopPerching && AIState == (float)BellbirdStates.Perching)
            {
                AIState = (float)BellbirdStates.Flying;
                Timer = 0f;
                LocalAIState = 0f;
                Projectile.netUpdate = true;
            }
        }

        public void DoBehavior_Flying()
        {
            // Float around the player.
            Projectile.FloatingPetAI(true, 0.03f);

            // After some time has passed, return to perching.
            if (Timer >= TimeToPerch)
            {
                AIState = (float)BellbirdStates.Perching;
                Timer = 0f;
                LocalAIState = 0f;
                Projectile.netUpdate = true;
            }

            // Teleport if too far away.
            float distanceFromOwner = Vector2.Distance(Owner.Center, Projectile.Center);
            if (distanceFromOwner > 2000f)
            {
                Projectile.Center = Owner.Center;
                Projectile.netUpdate = true;
            }

            // Only incremenet if gravity is normal.
            if (Owner.gravDir != -1f)
                Timer++;
            Projectile.UpdateProjectileAnimationFrames(0, 3, 4);
        }

        public void DoBehavior_Perching()
        {
            Vector2 perchPosition = Owner.RotatedRelativePoint(Owner.MountedCenter) + new Vector2(Owner.direction < 0 ? -6f : 6f, -2f);
            if (LocalAIState == 0f)
            {
                if (Projectile.Distance(perchPosition) <= 10f)
                {
                    LocalAIState = 1f;
                    Projectile.netUpdate = true;
                }

                Projectile.SimpleMove(perchPosition, 15f, 20f);
                Projectile.UpdateProjectileAnimationFrames(0, 3, 4);
                Projectile.rotation = Projectile.velocity.X * 0.03f;
            }

            if (LocalAIState == 1f)
            {
                Projectile.velocity *= 0f;
                Projectile.Center = perchPosition.Floor();
                Projectile.rotation = Owner.fullRotation;
                Projectile.direction = Owner.direction;
                Projectile.UpdateProjectileAnimationFrames(4, 4, 1);
            }
        }

        public void DoBehavior_CryOfGod()
        {
            ref float screamChargeVisualScale = ref Projectile.TwilightEgress().ExtraAI[ScreamChargeVisualScaleIndex];
            ref float screamChargeVisualOpacity = ref Projectile.TwilightEgress().ExtraAI[ScreamChargeVisualOpacityIndex];

            // Launch away from the player quickly.
            if (Timer == 0f)
            {
                float xVel = Main.rand.NextFloat(-10f, 10f);
                float yVel = Main.rand.NextFloat(-6f, -4f);
                Projectile.velocity = Main.rand.NextVector2CircularEdge(xVel, yVel);
            }

            // Slow down and play a small visual effect.
            if (Timer <= ScreamChargeTime)
            {
                Projectile.UpdateProjectileAnimationFrames(2, 2, 1);
                float screamChargeInterpolant = Utils.GetLerpValue(0f, ScreamChargeTime - 39, Timer, true);

                // Fade in then quickly fade out.
                if (Timer <= ScreamChargeTime - 30)
                {
                    screamChargeVisualScale = MathHelper.Lerp(5f, 1f, screamChargeInterpolant);
                    screamChargeVisualOpacity = MathHelper.Lerp(0f, 1f, screamChargeInterpolant);
                }

                if (Timer >= ScreamChargeTime - 15 && Timer <= ScreamChargeTime)
                    screamChargeVisualOpacity = MathHelper.Clamp(screamChargeVisualOpacity - 0.1f, 0f, 1f);
            }

            // Cry of God.
            if (Timer is >= ScreamChargeTime and <= ScreamChargeTime + ScreamTime)
            {
                // Make the player's ears bleed.
                if (Timer is ScreamChargeTime)
                    SoundEngine.PlaySound(AssetRegistry.Sounds.BellbirdStunningScream with { Volume = 30f }, Projectile.Center);

                // Visual effects.
                //TwilightEgressCameraSystem.Screenshake(8, 30, Projectile.Center);
                ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 8f, shakeStrengthDissipationIncrement: 0.26f, intensityTaperEndDistance: 2000);
                Projectile.UpdateProjectileAnimationFrames(0, 0, 1);
                if (Timer % 10 == 0)
                    new RoaringShockwaveParticle(45, Projectile.Center, Vector2.Zero, Color.White, 0.1f, Main.rand.NextFloat(MathHelper.TwoPi)).Spawn();

                // Stun any nearby NPCs or Players.
                StunPlayersAndNPCs();
            }

            if (Timer is >= ScreamChargeTime + ScreamTime)
            {
                AIState = (float)BellbirdStates.Flying;
                Timer = 0f;
                Projectile.netUpdate = true;
            }

            Timer++;
            Projectile.velocity *= 0.98f;
        }

        public void StunPlayersAndNPCs()
        {
            float maxDistance = 2500f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && npc.Distance(Projectile.Center) < maxDistance)
                    npc.AddBuff(ModContent.BuffType<BellbirdCry>(), 180);
            }

            for (int i = 0; i < Main.CurrentFrameFlags.ActivePlayersCount; i++)
            {
                Player player = Main.player[i];
                if (player.active && player.Distance(Projectile.Center) < maxDistance)
                    player.AddBuff(ModContent.BuffType<BellbirdCry>(), 180);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Texture2D chargeVisualTexture = ModContent.Request<Texture2D>(GlowTexture).Value;

            SpriteEffects directionEffects = Projectile.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            SpriteEffects effects = AIState == 1f && LocalAIState == 1f ? Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None
                : directionEffects;

            ref float screamChargeVisualScale = ref Projectile.TwilightEgress().ExtraAI[ScreamChargeVisualScaleIndex];
            ref float screamChargeVisualOpacity = ref Projectile.TwilightEgress().ExtraAI[ScreamChargeVisualOpacityIndex];

            int individualFrameHeight = texture.Height / Main.projFrames[Type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle rectangle = new(0, currentYFrame, texture.Width, individualFrameHeight);
            Vector2 origin = rectangle.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            Main.EntitySpriteDraw(texture, drawPosition, rectangle, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, effects);
            Main.EntitySpriteDraw(chargeVisualTexture, drawPosition, rectangle, Color.White * screamChargeVisualOpacity, Projectile.rotation, origin, screamChargeVisualScale, directionEffects);
            return false;
        }
    }
}
