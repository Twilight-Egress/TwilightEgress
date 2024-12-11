using Microsoft.Xna.Framework.Graphics;
using TwilightEgress.Content.Items.Dedicated.Fluffy;
using TwilightEgress.Content.Particles;

namespace TwilightEgress.Content.EntityOverrides.Items.ChickenCannon
{
    public class ChickenCannonMiniBoom : Bastsplosion, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";

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

        public override void OnSpawn(IEntitySource source)
        {
            // Spawn a pulse ring particle.
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0.2f, 0.8f));
            PulseRingParticle explosionRing = new(Projectile.Center, Vector2.Zero, color, 0.01f, 2f, new Vector2(1f, 1f), Main.rand.NextFloat(MathHelper.TwoPi), 75);
            explosionRing.SpawnCasParticle();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D value = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle val = value.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
            Vector2 origin = val.Size() / 2f;
            Color color = Color.Lerp(Color.Yellow, Color.Red, Main.rand.NextFloat(0.2f, 0.8f));
            Main.EntitySpriteDraw(value, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), val, Projectile.GetAlpha(color), Projectile.rotation, origin, Projectile.scale, 0);

            return false;
        }
    }
}
