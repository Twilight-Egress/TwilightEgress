using CalamityMod.Projectiles.Magic;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.Drawing;
using TwilightEgress.Core.Systems;
using static TwilightEgress.TwilightEgressUtilities;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthDirt : ModTile
    {
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            DustType = DustID.Dirt;

            AddMapEntry(new Color(75, 32, 51));

            glowTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Tiles/EnchantedOvergrowth/OvergrowthDirt_Glow");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);

            if (!TileDrawing.IsVisible(tile))
                return;

            Vector2 drawPosition = new Vector2(i * 16, j * 16) - Main.screenPosition;

            if (!Main.drawToScreen)
                drawPosition += new Vector2(Main.offScreenRange);

            Color drawColor = Color.White * (float)(1 + NoiseSystem.cellular.GetNoise(i * 4, j * 4, Main.GameUpdateCount));
            Color lightColor = Lighting.GetColor(i, j);

            drawColor.R = (byte)Math.Clamp(drawColor.R + lightColor.R, 0, 255);
            drawColor.G = (byte)Math.Clamp(drawColor.G + lightColor.G, 0, 255);
            drawColor.B = (byte)Math.Clamp(drawColor.B + lightColor.B, 0, 255);

            spriteBatch.Draw(glowTexture.Value, drawPosition, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }
    }
}
