namespace Cascade.Content.Items.Weapons.Rogue.TimelessCascade;

public class TimelessCascadeProj : ModProjectile
{
    
        public float explosionDamageMod = 0.95f;

        Vector2? saveVel = null;
    
        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = -1;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.ai[0] = Projectile.ai[0] > 0 ? 0 : Projectile.ai[0];

            Projectile.ai[2] = 10;
            Projectile.netUpdate = true;
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[2] == 0)
            { //set
                Projectile.ai[2] = 10;
            }


            Projectile.netUpdate = true;
        }


        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.Calamity().stealthStrike)
                modifiers.FinalDamage += 0.9f;
            if (Projectile.ai[2] == 0)
            { //set
                Projectile.ai[2] = 10;
                Projectile.netUpdate = true;
            }
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (saveVel == null && Projectile.ai[0] >= 0)
            {
                Projectile.ai[0] = 1;
                Projectile.ai[2] = 0;
                saveVel = Projectile.velocity;
            }
            
            if (Math.Abs(Projectile.ai[2] - 10) < .01f && Projectile.ai[1] != 1)
            {
                int proj = Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TimelessCascadeExplode>(), (int)(Projectile.damage * explosionDamageMod), Projectile.knockBack, Projectile.owner);
                Main.projectile[proj].rotation = Projectile.rotation;

                
                Projectile.Kill();

 
            }



            Projectile.rotation += .07f;


            Projectile.ai[0] -= .01f;

            if (Projectile.ai[0] < 0)
            {
                Projectile.tileCollide = false;
                Projectile.velocity = (-Projectile.Center + owner.Center).SafeNormalize(Vector2.Zero) * 10 * -(Projectile.ai[0]);

                Projectile.Opacity -= 0.005f;


                if (Projectile.Distance(owner.Center) < 20)
                    Projectile.Kill();

            }
            else
            {
                Projectile.velocity = (Vector2)saveVel * Projectile.ai[0];
            }
          
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return true;
        }
}