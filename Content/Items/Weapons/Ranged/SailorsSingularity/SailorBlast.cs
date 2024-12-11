using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Weapons.Ranged.SailorsSingularity
{
    public class SailorBlast : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 5;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }
        public override void AI()
        {
            Projectile.rotation += 0.15f;
            Lighting.AddLight(Projectile.Center, Color.BlueViolet.ToVector3());
        }
    }
}
