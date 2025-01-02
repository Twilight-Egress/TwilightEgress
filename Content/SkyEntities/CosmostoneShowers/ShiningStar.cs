using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using TwilightEgress.Core;

namespace TwilightEgress.Content.SkyEntities.CosmostoneShowers
{
    public class ShiningStar : SkyEntity
    {
        public readonly List<string> FourPointedStars_Atlas =
        [
            "TwilightEgress.FourPointedStar_Small.png",
            "TwilightEgress.FourPointedStar_Small_2.png",
            "TwilightEgress.FourPointedStar_Medium.png",
            "TwilightEgress.FourPointedStar_Medium_2.png",
            "TwilightEgress.FourPointedStar_Large.png",
            "TwilightEgress.FourPointedStar_Large_2.png"
        ];

        public float MaxScale;

        public float MinScale;

        public Vector2 StretchFactor;

        private readonly int TextureIndex;

        public const int BaseLifespan = 480;

        public ShiningStar(Vector2 position, Color color, float maxScale, float depth, Vector2 stretchFactor, int lifespan)
        {
            Position = position;
            Color = color;
            MaxScale = maxScale;
            MinScale = maxScale * 0.5f;
            StretchFactor = stretchFactor;
            Lifetime = lifespan + BaseLifespan;
            Depth = depth;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(MathHelper.TwoPi);
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

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override BlendState BlendState => BlendState.Additive;

        public override SkyEntityDrawContext DrawContext => SkyEntityDrawContext.AfterCustomSkies;

        public override void Update()
        {
            int timeToDisappear = Lifetime - 120;
            int timeToAppear = 120;
            float appearInterpolant = Time / (float)timeToAppear;
            float twinkleInterpolant = EasingFunctions.SineEaseInOut(Time / 60f);
            float disappearInterpolant = (Time - timeToDisappear) / 120f;

            Scale = new Vector2(MathHelper.Lerp(MinScale, MaxScale, twinkleInterpolant));

            if (Time <= timeToAppear)
                Opacity = MathHelper.Lerp(0f, 1f, appearInterpolant);
            if (Time >= timeToDisappear && Time <= Lifetime)
                Opacity = MathHelper.Lerp(Opacity, 0f, disappearInterpolant);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            AtlasTexture starTextures = AtlasManager.GetTexture(FourPointedStars_Atlas[TextureIndex]);
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            Vector2 mainOrigin = starTextures.Size / 2f;
            Vector2 bloomOrigin = bloomTexture.Size / 2f;

            Vector2 scaleWithDepth = Scale / Depth;
            Color color = Color * Opacity;

            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color, Rotation, bloomOrigin, scaleWithDepth / 8f);
            spriteBatch.Draw(starTextures, GetDrawPositionBasedOnDepth(), null, Color.White * Opacity, 0f, mainOrigin, scaleWithDepth * StretchFactor * 0.6f);
            spriteBatch.Draw(starTextures, GetDrawPositionBasedOnDepth(), null, color, 0f, mainOrigin, scaleWithDepth * StretchFactor);
        }
    }
}
