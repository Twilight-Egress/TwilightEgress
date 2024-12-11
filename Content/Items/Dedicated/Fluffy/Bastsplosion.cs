using Microsoft.Xna.Framework.Graphics;
using System;

namespace TwilightEgress.Content.Items.Dedicated.Fluffy
{
    public class Bastsplosion : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = Main.projFrames[645];
        }

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        public override void AI()
        {
            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;

                // Play the sound and pick a random rotation.
                SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
                Projectile.rotation = Main.rand.NextFloat((float)Math.PI * 2f);

                // Resize the hitbox.
                Projectile.scale = Main.rand.NextFloat(1f, 1.75f);
                Projectile.position = Projectile.Center;
                Projectile.width = (int)(Projectile.width * Projectile.scale);
                Projectile.height = (int)(Projectile.height * Projectile.scale);
                Projectile.Center = Projectile.position;

                Projectile.netUpdate = true;
            }

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame--;
                    Projectile.Kill();
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle val = value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = val.Size() / 2f;
            Color color = Projectile.GetAlpha(lightColor);
            Main.EntitySpriteDraw(value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), val, color, Projectile.rotation, origin, Projectile.scale, 0);

            return false;
        }
    }
}
