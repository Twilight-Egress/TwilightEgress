using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class TimedSmokeParticle : CasParticle
    {
        public Color DrawColorFire;

        public Color DrawColorFade;

        public float InitialOpacity;

        public int FrameVariant;

        public override string AtlasTextureName => "TwilightEgress.MediumSmoke.png";

        public override int FrameCount => 3;

        public TimedSmokeParticle(Vector2 position, Vector2 velocity, Color drawColorFire, Color drawColorFade, float scale, float initialOpacity, int lifetime, float rotationSpeed = 0f)
        {
            Position = position;
            Velocity = velocity;
            DrawColorFire = drawColorFire;
            DrawColorFade = drawColorFade;
            Scale = new(scale);
            InitialOpacity = initialOpacity;
            Lifetime = lifetime;
            RotationSpeed = rotationSpeed;

            FrameVariant = Main.rand.Next(FrameCount);
        }

        public override void Update()
        {
            // Spin and slow down.
            Rotation += RotationSpeed * Velocity.X.DirectionalSign();
            Velocity *= 0.85f;

            Opacity = MathHelper.Lerp(InitialOpacity, 0f, LifetimeRatio);
            if (LifetimeRatio < 0.75f)
                Scale += Vector2.One * 0.01f;
            else
                Scale *= 0.975f;

            // Lerp between the two colors depending on the lifetime of the particle.
            DrawColor = Color.Lerp(DrawColorFire, DrawColorFade, LifetimeRatio);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int individualFrameHeight = Texture.Height / FrameCount;
            int yFrameSubdivision = individualFrameHeight * FrameVariant;
            Rectangle frame = new(0, yFrameSubdivision, Texture.Width, individualFrameHeight);
            spriteBatch.Draw(Texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, frame.Size() / 2f, Scale);
        }
    }
}
