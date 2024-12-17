using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;

namespace TwilightEgress.Content.NPCs.CosmostoneShowers
{
    public class ManaInk : ModProjectile
    {
        public override string LocalizationCategory => "NPCs.CosmostoneShowers.Manaphage.Projectiles";

        public ref float Timer => ref Projectile.ai[0];

        public override string Texture => "TwilightEgress/Assets/Textures/Extra/EmptyPixel";

        public override void SetDefaults()
        {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.hostile = true;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 30;
        }

        public override void AI()
        {
            Projectile.velocity *= 0.98f;

            Projectile.scale = MathHelper.Lerp(0.2f, 2.5f, Timer / 30f);
            Projectile.Opacity = MathHelper.Lerp(1f, 0f, (Timer - 20f) / 10f);
            Projectile.AdjustProjectileHitboxByScale(30f, 30f);

            Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
            Color inkColor = Color.Lerp(Color.DarkBlue, Color.MidnightBlue, Main.rand.NextFloat(0.1f, 0.9f));

            new ManaInkParticle(spawnPosition, inkColor, Projectile.scale, 0.6f, 20).SpawnCasParticle();

            Timer++;
        }
    }
}
