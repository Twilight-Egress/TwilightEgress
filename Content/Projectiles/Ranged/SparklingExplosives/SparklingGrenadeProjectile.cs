using Terraria;

namespace TwilightEgress.Content.Projectiles.Ranged.SparklingExplosives
{
    public class SparklingGrenadeProjectile : ModProjectile
    {
        public override string Texture => "TwilightEgress/Content/Items/Weapons/Ranged/SparklingExplosives/SparklingGrenade";

        private List<Vector2> mousePositions = new List<Vector2>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.damage = 65;
            Projectile.knockBack = 8f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 360;

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
            // Rotation increased by velocity
            Projectile.rotation += 0.05f;

            if (Projectile.owner != Main.myPlayer)
                return;

            if (Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();
            else {
                Projectile.netUpdate = true;

                if (mousePositions.Count == 0 || ((Main.MouseScreen + Main.screenPosition) - mousePositions.Last()).LengthSquared() >= Pow(16f * 8f, 2))
                    mousePositions.Add(Main.MouseScreen + Main.screenPosition);

                if ((mousePositions.First() - Projectile.Center).LengthSquared() < Pow(16f * 4f, 2))
                    mousePositions.RemoveAt(0);

                List<Vector2> positionsToGoTo = new List<Vector2>();
                positionsToGoTo.AddRange(mousePositions);
                positionsToGoTo.Add(Main.MouseScreen + Main.screenPosition);

                Vector2 toFirstPos = positionsToGoTo.First() - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toFirstPos, 0.1f).SafeNormalize(Vector2.Zero) * 10f;

                if ((positionsToGoTo.First() - Projectile.Center).LengthSquared() < Pow(16f * 1f, 2))
                    Projectile.timeLeft = 3;
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

            Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3());

            Projectile.Resize(15, 15);
        }
    }
}
