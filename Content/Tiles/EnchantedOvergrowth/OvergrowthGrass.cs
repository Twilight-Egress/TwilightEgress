using TwilightEgress.Content.Items.Placeable.EnchantedOvergrowth;
using static TwilightEgress.TwilightEgressUtilities;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthGrass : ModTile
    {
        private Asset<Texture2D> grassTexture;

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
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail || effectOnly)
                return;

            Main.tile[i, j].TileType = (ushort)ModContent.TileType<OvergrowthDirt>();
        }

        public override bool CanDrop(int i, int j)
        {
            return Main.rand.Next(1, 3) == 1;
        }

        public override IEnumerable<Item> GetItemDrops(int i, int j)
        {
            Vector2 worldPosition = new Vector2(i, j).ToWorldCoordinates();

            int seedsItem = ModContent.ItemType<OvergrowthGrassSeeds>();

            yield return new Item(ModContent.ItemType<OvergrowthGrassSeeds>(), 1);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b) => (r, g, b) = (0.075f, 0.125f, 1.000f);

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
            AdjacencyData<Tile> adjacencyData = GetAdjacentTiles(i, j);

            bool leftSame = adjacencyData.left.TileType == Type;
            bool rightSame = adjacencyData.right.TileType == Type;

            List<Point> framesToDraw =
            [
                new Point(0, 0),
                new Point(0, 1),
                new Point(0, -1),
            ];

            if (!rightSame) 
            {
                framesToDraw.Add(new Point(1, 0));
                framesToDraw.Add(new Point(1, 1));
                framesToDraw.Add(new Point(1, -1));
            }
            
            if (!leftSame)
            {
                framesToDraw.Add(new Point(-1, 0));
                framesToDraw.Add(new Point(-1, 1));
                framesToDraw.Add(new Point(-1, -1));
            }

            foreach (Point frame in framesToDraw) 
            {
                int offsetX = (!rightSame && !leftSame) ? 180 + 54 * (i % 2) : 90 * (i % 2);
                int offsetY = 54 * (i % 3);

                if (leftSame)
                    offsetX += rightSame ? 18 : 36;

                Rectangle sourceRectangle = new Rectangle(offsetX + (frame.X + 1) * 18, offsetY + (frame.Y + 1) * 18, 16, 16);
                spriteBatch.DrawTileTexture(grassTexture.Value, i + frame.X, j + frame.Y, sourceRectangle, paintColor, 0f, Vector2.Zero, lighted: false);
            }
        }
    }
}
