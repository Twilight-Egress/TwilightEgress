namespace Cascade.Content.Items.Weapons.Rogue.TimelessCascade;

public class TimelessCascadeExplode : ModProjectile
{
    public override string Texture => "CalamityMod/Projectiles/InvisibleProj";
    
    
            public override void SetDefaults()
        {
            Projectile.width = 90;
            Projectile.height = 90;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 5; // 5 in-game frames per sprite frame. 12 sprite frames 
            Projectile.penetrate = -1;

        }

        bool runOnce = true;

        public override void AI()
        {
            Projectile.ai[0]++;

            if (runOnce)
            {
                runOnce = false;
                SoundEngine.PlaySound(SoundID.NPCDeath14.WithVolumeScale(.8f), Projectile.Center);
                float dustAmt = 48;
                for (int i = 1; i < dustAmt+1; i++)
                {
                    Vector2 circular = new Vector2(1.5f, 0).RotatedBy(MathHelper.ToRadians(i * 360 / dustAmt));
                    //circular.X *= 0.5f;
                    circular = circular.RotatedBy(MathF.PI / 2);
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Vector2 spawnPos = Projectile.Center + circular;

       
                        if (i % 6 == 0)
                        {
                            Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPos, circular, ModContent.ProjectileType<TimelessCascadeShards>(), (int)(Projectile.damage), Projectile.knockBack, Projectile.owner, ai0: i / 6, ai1:Projectile.Center.X, ai2:Projectile.Center.Y);
                        }
                  
                        if (i % 3 == 0)
                        {
                            Color color = Color.LightBlue;
                            float colorMod = i % 6 == 0 ? .76f : .35f;
                            float scale = 1;
                            int lifespan = 100;
                            float velMod = i % 6 == 0 ? 4.5f : 4;
            
                            SparkleParticle sparkleParticle = new(spawnPos, circular* velMod, color, color * colorMod, scale, lifespan, 0.25f, 1.25f);
                            sparkleParticle.SpawnCasParticle(); 
                        }
                        spawnPos = Projectile.Center + circular;
                    }
                }
            }

            Projectile.frame = (int)Math.Floor(Projectile.ai[0] / 35 * 7);
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(Color.White, Color.DarkSeaGreen, Projectile.ai[0] / 35);
        }
}