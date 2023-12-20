﻿using Cascade.Core.Graphics.GraphicalObjects.SkyEntitySystem;

namespace Cascade.Content.Skies.SkyEntities
{
    public class StationaryAsteroid : SkyEntity
    {
        public float RotationSpeed;

        public float RotationDirection;

        public override string TexturePath => "Cascade/Content/Projectiles/Ambient/Comet";

        public override int MaxFrames => 3;

        public override bool DieWithLifespan => true;

        public StationaryAsteroid(Vector2 position, float scale, float depth, float rotationSpeed, int lifespan)
        {
            Position = position;
            Scale = scale;
            Depth = depth;
            RotationSpeed = rotationSpeed;
            Lifespan = lifespan;

            Opacity = 0f;
            Frame = Main.rand.Next(3);
            Rotation = Main.rand.NextFloat(TwoPi);
            RotationDirection = Main.rand.NextBool().ToDirectionInt();
        }

        public override void Update()
        {
            int timeToDisappear = Lifespan - 60;

            // Fade in and out.
            if (Time < timeToDisappear)
                Opacity = Clamp(Opacity + 0.1f, 0f, 1f);
            if (Time >= timeToDisappear && Time <= Lifespan)
                Opacity = Clamp(Opacity - 0.1f, 0f, 1f);

            Rotation += RotationSpeed * RotationDirection;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle frameRectangle = StoredTexture.Frame(1, MaxFrames, 0, Frame % 3);
            Vector2 mainOrigin = frameRectangle.Size() / 2f;

            Color color = Color.Lerp(Color.White, Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.3f + (Depth / 10f)), 0.15f + (Depth / 10f)) * Opacity;
            spriteBatch.Draw(StoredTexture, GetDrawPositionBasedOnDepth(), frameRectangle, color, Rotation, mainOrigin, Scale / Depth, 0, 0f);
        }
    }
}