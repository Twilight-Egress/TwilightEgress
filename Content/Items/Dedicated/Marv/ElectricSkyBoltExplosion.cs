using CalamityMod.Sounds;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;

namespace TwilightEgress.Content.Items.Dedicated.Marv
{
    public class ElectricSkyBoltExplosion : ModProjectile
    {
        public ref float Timer => ref Projectile.ai[0];

        public override string LocalizationCategory => "Items.Dedicated.ThunderousFury.Projectiles";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = (int)(100f / Projectile.scale);
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.scale = 0.1f;
            Projectile.timeLeft = 60;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override void AI()
        {
            if (Timer >= 35f)
            {
                Projectile.Kill();
                return;
            }

            // Sounds and other initial effects.
            if (Timer == 1)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.ExoPlasmaExplosionSound);
                // Particle creation.
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int sparkLifespan = Main.rand.Next(20, 36);
                    float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                    Color sparkColor = Color.Lerp(Color.LightYellow, Color.Goldenrod, Main.rand.NextFloat());
                    for (int i = 0; i < 50; i++)
                    {
                        Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 16f);
                        SparkParticle electricSpark = new(Projectile.Center, sparkVelocity, sparkColor, sparkScale, sparkLifespan);
                        electricSpark.SpawnCasParticle();
                    }
                }
            }

            float sine = EasingFunctions.SineEaseInOut(Timer / 17.5f);
            if (Timer <= 35f)
            {
                Projectile.scale = MathHelper.Lerp(0.1f, 2f, sine);
                Projectile.Opacity = MathHelper.Lerp(0f, 1f, sine);
            }

            Timer++;
            ScreenShakeSystem.StartShakeAtPoint(Projectile.Center, 4f, shakeStrengthDissipationIncrement: 0.185f);
            Projectile.rotation += MathHelper.Pi / 30f;
            Lighting.AddLight(Projectile.Center, Color.Goldenrod.ToVector3() * 0.65f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D Vortex = AssetRegistry.Textures.GreyscaleObjects.GreyscaleVortex.Value;
            Color electroColor = Color.Lerp(Color.Goldenrod, Color.LightYellow, 0.35f);
            // Vortex 1.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(electroColor), -Projectile.rotation * 1.5f, Projectile.scale * 4f, texture: Vortex);
            //Vortex 2.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(electroColor), Projectile.rotation, Projectile.scale * 4.25f, texture: Vortex);
            // Electric Ball.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(electroColor), 0f, Projectile.scale, animated: true);

            return false;
        }
    }
}
