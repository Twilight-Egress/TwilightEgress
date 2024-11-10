using TwilightEgress.Core.Graphics.GraphicalObjects.SkyEntities;

namespace TwilightEgress.Content.Skies.SkyEntities
{
    public class Sirius : SkyEntity
    {
        public float MaxScale;

        public float MinScale;

        public const int BaseLifespan = 2400;

        public Sirius(Vector2 position, Color color, float maxScale, int lifespan)
        {
            Position = position;
            Color = color;
            MaxScale = maxScale;
            MinScale = maxScale * 0.75f;
            Lifetime = lifespan + BaseLifespan;
            Depth = 150f;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationSpeed = Main.rand.NextFloat(0.0025f, 0.01f);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override SkyEntityDrawContext DrawContext => SkyEntityDrawContext.AfterCustomSkies;

        public override BlendState BlendState => BlendState.Additive;

        public override void Update()
        {
            int timeToDisappear = Lifetime - 120;
            int timeToAppear = 120;
            float appearInterpolant = Time / (float)timeToAppear;
            float twinkleInterpolant = TwilightEgressUtilities.SineEaseInOut(Time / 120f);
            float disappearInterpolant = (Time - timeToDisappear) / 120f;

            Scale = new Vector2(Lerp(MinScale, MaxScale, twinkleInterpolant));

            if (Time <= timeToAppear)
                Opacity = Lerp(0f, 1f, appearInterpolant);
            if (Time >= timeToDisappear && Time <= Lifetime)
                Opacity = Lerp(Opacity, 0f, disappearInterpolant);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D sirius = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/SiriusMinion").Value;
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            Vector2 mainOrigin = sirius.Size() / 2f;
            Vector2 bloomOrigin = bloomTexture.Size / 2f;

            Color color = Color * Opacity;

            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color * 0.5f, Rotation, bloomOrigin, Scale / 5f);
            spriteBatch.Draw(bloomTexture, GetDrawPositionBasedOnDepth(), null, color * 0.7f, -Rotation, bloomOrigin, Scale / 3f);

            spriteBatch.Draw(sirius, GetDrawPositionBasedOnDepth(), null, Color.White * Opacity, 0f, mainOrigin, Scale, 0, 0f);
        }
    }
}
