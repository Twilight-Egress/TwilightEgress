namespace TwilightEgress.Content.Items.Weapons.Ranged.SparklingExplosives
{
    public class SparklingGrenadeProjectile : ModProjectile
    {
        public override string Texture => "TwilightEgress/Content/Items/Weapons/Ranged/SparklingExplosives/SparklingGrenade";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;

            Projectile.timeLeft = 360;

            Projectile.damage = 65;
            Projectile.knockBack = 8f;
            Projectile.DamageType = DamageClass.Ranged;

            DrawOffsetX = -1;
            DrawOriginOffsetY = -4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 0;
            Projectile.PrepareBombToBlow();
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                Projectile.timeLeft = 0;
                Projectile.PrepareBombToBlow();
                return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            if (Projectile.owner == Main.myPlayer && Projectile.timeLeft <= 3)
            {
                Projectile.PrepareBombToBlow();
            }

            // Angle towards cursor
            if (Projectile.timeLeft > 180)
            {
                float length = Projectile.velocity.Length();
                float targetAngle = Projectile.AngleTo(Main.screenPosition + Main.MouseScreen);
                Projectile.velocity = Projectile.velocity.ToRotation().AngleTowards(targetAngle, ToRadians(3)).ToRotationVector2() * length;
                Projectile.netUpdate = true;
            }

            // Rotation increased by velocity
            Projectile.rotation += 0.05f;
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.Resize(60, 60);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // spawn sparkle particles
            for (int i = 0; i < 10; i++)
            {
                new SparkleParticle(Projectile.Center, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(6), Color.LightBlue, Color.DarkBlue, Main.rand.NextFloat(0.2f, 0.5f), 20, bloomScale: 0f).SpawnCasParticle();
            }

            Projectile.Resize(15, 15);
        }
    }
}
