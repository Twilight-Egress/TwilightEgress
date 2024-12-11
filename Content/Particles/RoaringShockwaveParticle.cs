using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class RoaringShockwaveParticle : CasParticle
    {
        public override string AtlasTextureName => "TwilightEgress.DistortedShockwave2.png";

        public RoaringShockwaveParticle(int lifespan, Vector2 position, Vector2 velocity, Color color, float scale, float rotation = 1f)
        {
            Lifetime = lifespan;
            Position = position;
            Velocity = velocity;
            DrawColor = color;
            Scale = new(scale * 0.3f, scale * 0.3f);
            Rotation = rotation;
            Opacity = 1f;
        }

        public override void Update()
        {
            Scale += new Vector2(0.36f, 0.36f);
            Opacity = Lerp(1f, 0f, LifetimeRatio);

            if (Opacity <= 0f) { Kill(); }
        }
    }
}
