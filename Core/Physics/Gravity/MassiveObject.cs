using Terraria.DataStructures;
using TwilightEgress.Core.Physics.VerletIntegration;

namespace TwilightEgress.Core.Physics.Gravity
{
    public class MassiveObject : VerletObject
    {
        public float Mass;

        public MassiveObject(Vector2 position, Vector2 velocity, float radius, float rotationSpeed, float mass)
        {
            Position = position;
            OldPosition = position - velocity;
            Radius = radius;
            RotationSpeed = rotationSpeed;
            Mass = mass;
            Active = true;
        }

        public override void OnCollide(VerletObject collidedWith)
        {
            Vector2 midpoint = (collidedWith.Position - Position) * 0.5f + Position;
            int fireyMineBoom = Projectile.NewProjectile(new EntitySource_WorldEvent(), midpoint, Vector2.Zero, ProjectileID.ExplosiveBullet, 0, 0f);
            Main.projectile[fireyMineBoom].timeLeft = 0;

            Active = false;
            collidedWith.Active = false;
        }
    }
}
