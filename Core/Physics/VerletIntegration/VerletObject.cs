using Microsoft.Xna.Framework;

namespace TwilightEgress.Core.Physics.VerletIntegration
{
    public abstract class VerletObject
    {
        public float Radius { get; set; }

        public int WhoAmI { get; set; }

        public bool Active { get; set; }

        public Vector2 Position { get; set; }
        public Vector2 OldPosition { get; set; }
        public Vector2 Acceleration { get; set; }

        public float Rotation { get; set; }
        public float RotationSpeed { get; set; }

        public Vector2 Velocity => Position - OldPosition;

        public void UpdatePosition(float deltaTime)
        {
            Vector2 velocity = Position - OldPosition;
            OldPosition = Position;
            Position = Position + velocity + Acceleration * deltaTime * deltaTime;
            Acceleration = Vector2.Zero;
        }

        public void Accelerate(Vector2 acceleration)
        {
            Acceleration += acceleration;
        }

        public virtual void OnCollide(VerletObject collidedWith)
        {

        }
    }
}