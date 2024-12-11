using System;
using TwilightEgress.Core.Graphics.GraphicalObjects.SkyEntities;

namespace TwilightEgress.Content.Skies.SkyEntities.StationaryAsteroids
{
    public class StationaryCosmostoneGeode : SkyEntity
    {
        public StationaryCosmostoneGeode(Vector2 position, float scale, float depth, float rotationSpeed, int lifespan)
        {
            Position = position;
            Scale = new(scale);
            Depth = depth;
            RotationSpeed = rotationSpeed;
            Lifetime = lifespan;

            Opacity = 0f;
            Rotation = Main.rand.NextFloat(MathF.Tau);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override void Update()
        {
            int timeToDisappear = Lifetime - 60;

            // Fade in and out.
            if (Time < timeToDisappear)
                Opacity = Clamp(Opacity + 0.1f, 0f, 1f);
            if (Time >= timeToDisappear && Time <= Lifetime)
                Opacity = Clamp(Opacity - 0.1f, 0f, 1f);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D asteroid = ModContent.Request<Texture2D>("TwilightEgress/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneGeode").Value;
            Texture2D glow = ModContent.Request<Texture2D>("TwilightEgress/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneGeode_Glow").Value;

            Vector2 mainOrigin = asteroid.Size() / 2f;
            Color color = Color.Lerp(Color.White, Color.Black, 0.15f + Depth / 10f) * Opacity;
            Color glowMaskColor = Color.Lerp(Color.White, Color.Black, 0.05f + Depth / 10f) * Opacity;

            spriteBatch.Draw(asteroid, GetDrawPositionBasedOnDepth(), null, color, Rotation, mainOrigin, Scale / Depth, 0, 0f);
            spriteBatch.Draw(glow, GetDrawPositionBasedOnDepth(), null, glowMaskColor, Rotation, mainOrigin, Scale / Depth, 0, 0f);
        }
    }
}
