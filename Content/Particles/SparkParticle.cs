using Microsoft.Xna.Framework.Graphics;
using System;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class SparkParticle : CasParticle
    {
        public float InitialOpacity;

        public bool AffectedByGravity;

        public override string AtlasTextureName => "TwilightEgress.LightStreak.png";

        public override BlendState BlendState => BlendState.Additive;

        public SparkParticle(Vector2 position, Vector2 velocity, Color drawColor, float scale, int lifetime, bool affectedByGravity = false)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Scale = new(scale);
            Lifetime = lifetime;
            AffectedByGravity = affectedByGravity;
        }

        public override void Update()
        {
            Scale *= 0.95f;
            Opacity = MathHelper.Lerp(InitialOpacity, 0f, MathF.Pow(LifetimeRatio, 4f));

            Velocity *= 0.95f;
            if (Velocity.Length() < 12f && AffectedByGravity)
            {
                Velocity.X *= 0.94f;
                Velocity.Y += 0.25f;
            }

            Rotation = Velocity.ToRotation() + MathHelper.PiOver2;
        }
    }
}
