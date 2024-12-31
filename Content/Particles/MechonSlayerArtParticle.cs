using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
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

            List<string> ArtTexturePaths = new()
            {
                "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/ArmorArt",
                "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/EaterArt",
                "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/EnchantArt",
                "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/PurgeArt",
                "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/SpeedArt",
            };

            Texture2D artTexture = ModContent.Request<Texture2D>(ArtTexturePaths[ArtType]).Value;
            Vector2 drawPosition = Position - Main.screenPosition;
            spriteBatch.Draw(artTexture, drawPosition, null, Color.White * Opacity, 0f, artTexture.Size() / 2f, Scale, 0, 0f);
        }
    }
}
