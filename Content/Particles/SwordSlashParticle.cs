using Luminance.Common.Utilities;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class SwordSlashParticle : CasParticle
    {
        private float BaseScale;

        private float BloomOpacity;

        private Vector2 StretchFactor;

        private Color BloomColor;

        public override string AtlasTextureName => "TwilightEgress.LightStreak.png";

        public override BlendState BlendState => BlendState.Additive;

        public SwordSlashParticle(Vector2 position, Color slashColor, Color bloomColor, float rotation, Vector2 stretchFactor, float scale, int lifespan)
        {
            Position = position;
            DrawColor = slashColor;
            BloomColor = bloomColor;
            Rotation = rotation;
            StretchFactor = stretchFactor;
            BaseScale = scale;
            Lifetime = lifespan;
        }

        public override void Update()
        {
            Scale = new(Lerp(BaseScale, 0f, LifetimeRatio));
            BloomOpacity = Lerp(1f, 0f, LifetimeRatio);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Texture2D bloomTexture = ModContent.Request<Texture2D>("CalamityMod/UI/ModeIndicator/BloomFlare").Value;
            Vector2 drawPosition = Position - Main.screenPosition;

            spriteBatch.Draw(bloomTexture, drawPosition, null, BloomColor * BloomOpacity, Rotation, bloomTexture.Size() / 2f, Scale, SpriteEffects.None, 0f);  
            spriteBatch.Draw(Texture, drawPosition, null, DrawColor, Rotation, Texture.Frame.Size() / 2f, Scale * StretchFactor, SpriteEffects.None);
        }
    }
}
