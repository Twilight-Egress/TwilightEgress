using CalamityMod;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using System;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class SparkleParticle : CasParticle
    {
        public bool AdditiveBlending;

        public int VisualStyle;

        public Color BloomColor;

        public float BloomScale;

        private AtlasTexture StarTextureToDraw;

        public override string AtlasTextureName => "TwilightEgress.Sparkle.png";

        public override BlendState BlendState => AdditiveBlending ? BlendState.Additive : BlendState.AlphaBlend;

        public SparkleParticle(Vector2 position, Vector2 velocity, Color drawColor, Color bloomColor, float scale, int lifeTime, float rotationSpeed = 0f, float bloomScale = 1f, bool additiveBlending = true, int? visualStyle = null)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            BloomColor = bloomColor;
            Scale = new(scale);
            Lifetime = lifeTime;
            RotationSpeed = rotationSpeed;
            BloomScale = bloomScale;
            AdditiveBlending = additiveBlending;
            VisualStyle = visualStyle ?? Main.rand.NextBool().ToDirectionInt();
        }

        public override void Update()
        {
            // Fade in and out.
            Opacity = MathF.Sin(LifetimeRatio * MathHelper.Pi);

            // Slow down and spin.
            Velocity *= 0.98f;
            Rotation += RotationSpeed * Velocity.X.DirectionalSign();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            StarTextureToDraw = VisualStyle == -1 ? Texture : AtlasManager.GetTexture("TwilightEgress.Sparkle2.png");
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            spriteBatch.Draw(bloomTexture, Position - Main.screenPosition, null, BloomColor * Opacity * 0.5f, Rotation, null, Scale / 2f);
            spriteBatch.Draw(StarTextureToDraw, Position - Main.screenPosition, null, DrawColor * Opacity * 0.5f, Rotation, null, Scale * 0.75f);
            spriteBatch.Draw(StarTextureToDraw, Position - Main.screenPosition, null, DrawColor * Opacity, Rotation, null, Scale);
        }
    }
}
