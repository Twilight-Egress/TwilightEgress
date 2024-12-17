using Terraria.ModLoader;
using TwilightEgress.Content.Items.Dedicated.Fluffy;

namespace TwilightEgress.Content.Items.Dedicated.Jacob
{
    public class Tanksplosion : Bastsplosion
    {
        public override string LocalizationCategory => "Items.Dedicated.TomeOfTheTank.Projectiles";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }
    }
}
