using TwilightEgress.Assets;
using TwilightEgress.Core.Graphics.GraphicalObjects.Particles;

namespace TwilightEgress.Content.Particles
{
    public class ManaInkParticle : CasParticle
    {
        private readonly float BaseOpacity;

        private readonly Texture2D SmokeTexture;

        public ManaInkParticle(Vector2 position, Color color, float scale, float baseOpacity, int lifespan)
        {
            Position = position;
            DrawColor = color;
            Scale = new(scale);
            BaseOpacity = baseOpacity;
            Lifetime = lifespan;

            Rotation = Main.rand.NextFloat(TwoPi);
            SmokeTexture = ModContent.Request<Texture2D>(AssetRegistry.Textures.Smokes[Main.rand.Next(AssetRegistry.Textures.Smokes.Count)]).Value;
        }

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public override BlendState BlendState => BlendState.Additive;

        public override void Update()
        {
            Rotation += Velocity.X * 0.004f;
            Velocity *= 0.98f;

            int fadeOutThreshold = Lifetime - 10;
            Opacity = Lerp(BaseOpacity, 0f, (Time - fadeOutThreshold) / 10f);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 position = Position - Main.screenPosition;
            Vector2 origin = SmokeTexture.Size() / 2f;
            spriteBatch.Draw(SmokeTexture, position, SmokeTexture.Frame(), DrawColor * Opacity, Rotation, origin, Scale / 12f, 0, 0f);
        }
    }
}
