using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using System;
using Terraria.Audio;
using Terraria.ID;

namespace TwilightEgress.Content.Items.Accessories.Elementals.TwinGeminiGenies
{
    public class Sandnado : ModProjectile, ILocalizedModType
    {
        private ref float Timer => ref Projectile.ai[0];

        private ref float ColorPicker => ref Projectile.ai[1];

        private ref float SoundTracker => ref Projectile.localAI[1];

        public new string LocalizationCategory => "Projectiles.Summon";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 6;
            ProjectileID.Sets.TrailCacheLength[Type] = 13;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.scale = 0f;
            Projectile.Opacity = 0f;
            Projectile.timeLeft = 360;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
        }

        public override void AI()
        {
            float lifespan = 360f;
            SlotId loopingTwisterSound;

            Projectile.GetNearestTarget(1000f, 500f, out bool foundTarget, out NPC closestTarget);
            if (!foundTarget || GeminiGenieSandy.Myself is null)
            {
                Projectile.velocity *= 0.98f;
                Projectile.Opacity -= 0.01f;
                Projectile.scale -= 0.01f;
                Projectile.AdjustProjectileHitboxByScale(60f, 60f);
                if (Projectile.Opacity <= 0f || Projectile.scale <= 0f)
                {
                    Projectile.Kill();
                }
                return;
            }

            // Play the looping sound.
            if (Projectile.soundDelay == 0f)
            {
                Projectile.soundDelay = -1;
                loopingTwisterSound = SoundEngine.PlaySound(SoundID.DD2_BookStaffTwisterLoop, Projectile.Center);
                SoundTracker = loopingTwisterSound.ToFloat();
            }

            if (SoundEngine.TryGetActiveSound(SlotId.FromFloat(SoundTracker), out ActiveSound retrievedSound))
            {
                retrievedSound.Position = Projectile.Center;
                // Scale volume with the projectile's life time.
                retrievedSound.Volume = 1f - MathF.Max(Timer - (lifespan - 15f), 0f) / 15f;
            }
            else
            {
                loopingTwisterSound = SlotId.Invalid;
                SoundTracker = loopingTwisterSound.ToFloat();
            }

            if (Timer == 0f)
            {
                // Spawn a dust line between the caster and the projectile's center;
                int dustType = ColorPicker == 1f ? DustID.PurpleTorch : DustID.GoldCoin;
                for (int i = 0; i < 60; i++)
                {
                    Vector2 dustPosition = Vector2.Lerp(Projectile.Center, GeminiGenieSandy.Myself.Center, i / 60f);
                    Dust d = Dust.NewDustPerfect(dustPosition, dustType);
                    d.noGravity = true;
                }

                TwilightEgressUtilities.CreateDustCircle(36, Projectile.Center, dustType, 10f);
            }

            // Fade in.
            if (Timer <= 60f)
            {
                Projectile.velocity *= 0.98f;
                Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1f, TwilightEgressUtilities.SineEaseInOut(Timer / 60f));
                Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, TwilightEgressUtilities.SineEaseInOut(Timer / 60f));
            }

            // Home in on targets, though very sloppily.
            if (Timer >= 60f && Timer <= lifespan - 60f)
                Projectile.SimpleMove(closestTarget.Center, 25f, 75f);

            // Fade out.
            if (Timer >= lifespan - 60f && Timer <= lifespan)
            {
                Projectile.scale = MathHelper.Clamp(Projectile.scale - 0.02f, 0f, 1f);
                Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity - 0.02f, 0f, 1f);
            }

            Timer++;
            Projectile.rotation = Projectile.velocity.X * 0.03f;
            Projectile.UpdateProjectileAnimationFrames(0, 6, 3);
            Projectile.AdjustProjectileHitboxByScale(60f, 60f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color color = ColorPicker == 1f ? Color.Purple : Color.Gold;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Projectile.DrawBackglow(Projectile.GetAlpha(color * 0.45f), 2f);
            Utilities.DrawAfterimagesCentered(Projectile, 0, Projectile.GetAlpha(color));
            Main.spriteBatch.ResetToDefault();

            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(color), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }
    }
}
