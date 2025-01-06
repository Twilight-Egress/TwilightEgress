using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using TwilightEgress.Assets;
using TwilightEgress.Core;
using TwilightEgress.Core.Graphics.Particles;

namespace TwilightEgress.Content.Particles
{
    public class MechonSlayerArtParticle : CasParticle
    {
        private readonly float BaseScale;

        private readonly float NewScale;

        private readonly int ArtType;

        public override string AtlasTextureName => "TwilightEgress.EmptyPixel.png";

        public MechonSlayerArtParticle(Vector2 position, float baseScale, float newScale, int artType, int lifespan)
        {
            Position = position;
            BaseScale = baseScale;
            NewScale = newScale;
            ArtType = artType;
            Lifetime = lifespan;
        }

        public override void Update()
        {
            Opacity = MathHelper.Lerp(1f, 0f, LifetimeRatio);
            Scale = new(MathHelper.Lerp(BaseScale, NewScale, EasingFunctions.SineEaseOut(LifetimeRatio)));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (ArtType == -1)
                return;

            List<Texture2D> ArtTexturePaths = new()
            {
                AssetRegistry.Textures.Enchilada.ArmorArt.Value,
                AssetRegistry.Textures.Enchilada.EaterArt.Value,
                AssetRegistry.Textures.Enchilada.EnchantArt.Value,
                AssetRegistry.Textures.Enchilada.PurgeArt.Value,
                AssetRegistry.Textures.Enchilada.SpeedArt.Value,
            };

            Vector2 drawPosition = Position - Main.screenPosition;
            spriteBatch.Draw(ArtTexturePaths[ArtType], drawPosition, null, Color.White * Opacity, 0f, ArtTexturePaths[ArtType].Size() / 2f, Scale, 0, 0f);
        }
    }
}
