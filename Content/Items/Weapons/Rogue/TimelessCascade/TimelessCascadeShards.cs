using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cascade.Content.Items.Weapons.Rogue.TimelessCascade
{
    public class TimelessCascadeShards : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 8;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        Vector2? initVel = null;
        int spawnTime = 30;
        public override bool PreAI()
        {

            spawnTime--;

            Vector2 targetPos = new Vector2(Projectile.ai[1], Projectile.ai[2]);
            if (spawnTime < 0 && Projectile.Distance(targetPos) < 1)
            {
                Projectile.Kill();
            }

            initVel = initVel ?? Projectile.velocity;
            if (Projectile.ai[0] < 1)
                return false;
            Projectile.frame = (int)Projectile.ai[0]-1;
            return base.PreAI();
        }
        public override void AI()
        {
   
            Projectile.velocity -= (Vector2)initVel*0.01f;

            Projectile.rotation += Projectile.velocity.Length()*.01f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return Projectile.ai[0] > 0;
        }


    }
}
