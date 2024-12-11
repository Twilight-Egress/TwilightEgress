using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using TwilightEgress.Assets;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    internal class ShiningStarParticle : CasParticle
    {
        public float MaxScale;

        public float MinScale;

        public float RotationDirection;

        public Vector2 StretchFactor;

        private int TextureIndex;

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override BlendState BlendState => BlendState.Additive;

        public const int BaseLifespan = 480;

        public ShiningStarParticle(Vector2 position, Color color, float maxScale, float depth, Vector2 stretchFactor, int lifespan)
        {
            Position = position;
            DrawColor = color;
            MaxScale = maxScale;
            MinScale = maxScale * 0.5f;
            StretchFactor = stretchFactor;
            Lifetime = lifespan + BaseLifespan;
            ParallaxStrength = depth;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.0025f, 0.01f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();

            // Pick a different texture depending on the max scale of the star.
            if (MaxScale <= 1.5f)
                TextureIndex = Main.rand.Next(2);
            if (MaxScale is > 1.5f and <= 2f)
                TextureIndex = Main.rand.Next(2, 4);
            if (MaxScale is > 2f and <= 3f)
                TextureIndex = Main.rand.Next(4, 6);
        }

        public override void Update()
        {
            int timeToDisappear = Lifetime - 120;
            int timeToAppear = 120;
            float appearInterpolant = Time / (float)timeToAppear;
            float twinkleInterpolant = TwilightEgressUtilities.SineEaseInOut(Time / 120f);
            float disappearInterpolant = (Time - timeToDisappear) / 120f;

            Scale = new(Lerp(MinScale, MaxScale, twinkleInterpolant));

            if (Time <= timeToAppear)
                Opacity = Lerp(0f, 1f, appearInterpolant);
            if (Time >= timeToDisappear && Time <= Lifetime)
                Opacity = Lerp(Opacity, 0f, disappearInterpolant);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            AtlasTexture starTextures = AtlasManager.GetTexture(AssetRegistry.Textures.FourPointedStars_Atlas[TextureIndex]);
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            Vector2 mainOrigin = starTextures.Size / 2f;
            Vector2 bloomOrigin = bloomTexture.Size / 2f;

            Color color = DrawColor * Opacity;
            spriteBatch.Draw(bloomTexture, GetDrawPositionWithParallax(), null, color, Rotation, bloomOrigin, Scale / 8f);
            spriteBatch.Draw(starTextures, GetDrawPositionWithParallax(), null, Color.White * Opacity, 0f, mainOrigin, Scale * StretchFactor * 0.6f);
            spriteBatch.Draw(starTextures, GetDrawPositionWithParallax(), null, color, 0f, mainOrigin, Scale * StretchFactor);
        }
    }
}
