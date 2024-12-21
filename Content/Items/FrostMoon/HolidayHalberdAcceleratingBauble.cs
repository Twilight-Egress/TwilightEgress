using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Graphics;

namespace TwilightEgress.Content.Items.FrostMoon
{
    public class HolidayHalberdAcceleratingBauble : ModProjectile
    {
        public override string LocalizationCategory => "Items.FrostMoon.HolidayHalberd.Projectiles";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 7;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = Projectile.timeLeft <= 180;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.timeLeft = 240;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 6;

            Projectile.Opacity = 0f;
        }

        public override void AI()
        {
            // Accelerate over time.
            if (Projectile.velocity.Length() < 30f)
                Projectile.velocity *= 1.06f;
            else
                Projectile.velocity = Vector2.Normalize(Projectile.velocity) * Projectile.velocity.Length();

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.frame = Main.rand.Next(0, 2);
                Projectile.netUpdate = true;
            }

            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.04f, 0f, 1f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 vel = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.Clentaminator_Red, vel * 5f);
                d.noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Color drawColor = Projectile.frame == 0 ? Color.Red : Color.Green;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Utilities.DrawAfterimagesCentered(Projectile, 0, Projectile.GetAlpha(drawColor));
            Main.spriteBatch.ResetToDefault();

            Projectile.DrawBackglow(Projectile.GetAlpha(drawColor * 0.45f), 3f);
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(lightColor), Projectile.rotation, Projectile.scale, animated: true);

            return false;
        }
    }
}
