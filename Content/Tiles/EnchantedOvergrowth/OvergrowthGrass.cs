using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using TwilightEgress.Content.Items.Placeable.EnchantedOvergrowth;
using static TwilightEgress.TwilightEgressUtilities;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthGrass : ModTile
    {
        private Asset<Texture2D> grassTexture;
        private Asset<Texture2D> glowTexture;

        public override void SetStaticDefaults()
        {
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.BlockMergesWithMergeAllBlock[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlendAll[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            HitSound = SoundID.Grass;

            AddMapEntry(new Color(48, 77, 247));

            grassTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Tiles/EnchantedOvergrowth/OvergrowthGrass_Grass");
            glowTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Tiles/EnchantedOvergrowth/OvergrowthGrass_Glow");
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail || effectOnly)
                return;

            Main.tile[i, j].TileType = (ushort)ModContent.TileType<OvergrowthDirt>();
        }

        public override bool CanDrop(int i, int j) => Main.rand.Next(1, 3) == 1;

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();

            int seedsItem = ModContent.ItemType<OvergrowthGrassSeeds>();

            yield return new Item(ModContent.ItemType<OvergrowthGrassSeeds>(), 1);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.019f, 0.031f, 0.250f);

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            try
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
            catch { }
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // TO-DO: theres absolutely a better way to do this all lmao
            Color paintColor = WorldGen.paintColor(Framing.GetTileSafely(i, j).TileColor);
            AdjacencyData<bool> adjacencyData = GetAdjacentTiles(i, j, (tile) => tile.TileType == Type);

            int offsetX = (!adjacencyData.left && !adjacencyData.right) ? 180 + 54 * (i % 2) : 90 * (i % 2);
            int offsetY = 54 * (i % 3);

            if (adjacencyData.left)
                offsetX += adjacencyData.right ? 18 : 36;

            int offsetX = (!rightSame && !leftSame) ? 180 + 54 * (i % 2) : 90 * (i % 2);
            int offsetY = 54 * (i % 3);

            if (adjacencyData.left)
                offsetX += adjacencyData.right ? 18 : 36;

            List<Point> framesToDraw =
            [
                new Point(0, 0),
                new Point(0, 1),
                new Point(0, -1),
            ];

            if (!adjacencyData.right) 
            {
                framesToDraw.Add(new Point(1, 0));
                framesToDraw.Add(new Point(1, 1));
                framesToDraw.Add(new Point(1, -1));
            }
            
            if (!adjacencyData.left)
            {
                framesToDraw.Add(new Point(-1, 0));
                framesToDraw.Add(new Point(-1, 1));
                framesToDraw.Add(new Point(-1, -1));
            }

            foreach (Point frame in framesToDraw) 
            {
                Rectangle sourceRectangle = new Rectangle(offsetX + (frame.X + 1) * 18, offsetY + (frame.Y + 1) * 18, 16, 16);
                spriteBatch.DrawTileTexture(grassTexture.Value, i + frame.X, j + frame.Y, sourceRectangle, paintColor, 0f, Vector2.Zero);
                spriteBatch.DrawTileTexture(glowTexture.Value, i + frame.X, j + frame.Y, sourceRectangle, paintColor, 0f, Vector2.Zero, lighted: false);
            }
        }
    }
}
