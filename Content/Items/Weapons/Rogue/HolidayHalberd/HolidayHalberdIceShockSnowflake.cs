using CalamityMod;
using Luminance.Common.Utilities;

namespace TwilightEgress.Content.Items.Weapons.Rogue.HolidayHalberd
{
    public class HolidayHalberdIceShockSnowflake : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 3;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.timeLeft = 75;
            Projectile.scale = 1.75f;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            Projectile.frame = Main.rand.Next(3);
            Projectile.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
        }

        public override void AI()
        {
            Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.ai[0]);
            Projectile.rotation += MathHelper.Pi / 45f;
            Projectile.velocity *= 0.987f;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
            TwilightEgressUtilities.CreateRandomizedDustExplosion(12, Projectile.Center, DustID.IceTorch, dustScale: 5f);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Utilities.DrawAfterimagesCentered(Projectile, 0, Projectile.GetAlpha(Color.White));
            Main.spriteBatch.ResetToDefault();

            Projectile.DrawBackglow(Projectile.GetAlpha(Color.White * 0.45f), 3f);
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(lightColor), Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }
    }
}
