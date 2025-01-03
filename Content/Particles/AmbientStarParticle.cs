using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO.Pipelines;
using Terraria;
using TwilightEgress.Core.Graphics.Particles;

namespace TwilightEgress.Content.Particles
{
    public class AmbientStarParticle : CasParticle
    {
        public float InitialOpacity;

        public float MaxOpacity;

        public override string AtlasTextureName => "TwilightEgress.Sparkle.png";

        public override BlendState BlendState => BlendState.Additive;

        public AmbientStarParticle(Vector2 position, Vector2 velocity, float scale, float initialOpacity, float maxOpacity, float parallaxStrength, int lifetime, Color? drawColor = null)
        {
            Position = position;
            Velocity = velocity;
            Scale = new(scale);
            InitialOpacity = initialOpacity;
            MaxOpacity = maxOpacity;
            ParallaxStrength = parallaxStrength;
            Lifetime = lifetime;
            DrawColor = drawColor ?? Color.White;
        }

        public override void Update()
        {
            int fadeInThreshold = 30;
            int fadeOutThreshold = Lifetime - 30;

            if (Time <= fadeInThreshold)
                Opacity = MathHelper.Clamp(Opacity + 0.1f, InitialOpacity, MaxOpacity);
            if (Time >= fadeOutThreshold && Time <= Lifetime)
                Opacity = MathHelper.Clamp(Opacity - 0.1f, InitialOpacity, MaxOpacity);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 scale = Scale * (ParallaxStrength * 0.5f);
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            spriteBatch.Draw(bloomTexture, GetDrawPositionWithParallax(), null, DrawColor * 0.5f * Opacity * 0.5f, Rotation, null, scale * 0.08f);
            spriteBatch.Draw(TwilightEgress.Pixel, GetDrawPositionWithParallax(), TwilightEgress.Pixel.Bounds, DrawColor * Opacity, 0f, TwilightEgress.Pixel.Size() * 0.5f, scale * 6f, SpriteEffects.None, 0f);

        }
    }
}
