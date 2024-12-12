using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class MusicNoteParticle : CasParticle
    {
        private const int MusicNoteScaleChangeThreshold = 30;

        private const int BaseLifespan = 180;

        private Texture2D MusicNoteTexture;

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        /// <param name="lifespan">The lifespan of this particle defaults to 180 ticks (3 seconds).
        /// Modifying this value will simply add to that value.</param>
        public MusicNoteParticle(Vector2 position, Vector2 velocity, int lifespan = 0)
        {
            Position = position;
            Velocity = velocity;
            Lifetime = BaseLifespan + lifespan;

            List<string> MusicNoteTexturePaths = new()
            {
                "Terraria/Images/Projectile_76",
                "Terraria/Images/Projectile_77",
                "Terraria/Images/Projectile_78",
            };

            MusicNoteTexture = ModContent.Request<Texture2D>(MusicNoteTexturePaths[Main.rand.Next(3)]).Value;

            Scale = new(0f);
            Rotation = 0f;
        }

        public override void Update()
        {
            // Scale up a bit before scaling back down.
            if (Time is <= MusicNoteScaleChangeThreshold)
                Scale = new(MathHelper.Clamp(Scale.X + 0.04f, 0f, 1f));
            if (Time >= Lifetime - MusicNoteScaleChangeThreshold && Time <= Lifetime)
                Scale = new(MathHelper.Clamp(Scale.X - 0.04f, 0f, 1f));

            Velocity *= 0.98f;
            Rotation = MathHelper.Lerp(MathHelper.ToRadians(-15f), MathHelper.ToRadians(15f), EasingFunctions.SineEaseInOut(Time / 45f));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 drawPosition = Position - Main.screenPosition;
            spriteBatch.Draw(MusicNoteTexture, drawPosition, null, Color.White, Rotation, MusicNoteTexture.Size() / 2f, Scale, 0, 0f);
        }
    }
}
