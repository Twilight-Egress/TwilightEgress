using static TwilightEgress.TwilightEgressUtilities;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthGrass : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileMerge[Type][ModContent.TileType<OvergrowthDirt>()] = true;

            HitSound = SoundID.Grass;

            AddMapEntry(new Color(48, 77, 247));
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (!fail || effectOnly)
                return;

            Main.tile[i, j].TileType = (ushort)ModContent.TileType<OvergrowthDirt>();
        }

        public override void DrawEffects(int i, int j, SpriteBatch spriteBatch, ref TileDrawInfo drawData)
        {
            try
            {
                Main.instance.TilesRenderer.AddSpecialLegacyPoint(i, j);
            }
            catch { }
        }

        private void DrawExtra(SpriteBatch spriteBatch, Texture2D grassTexture, int i, int j, Color paintColor, Rectangle sourceRectangle)
        {
            Vector2 drawOffset = new Vector2(i * 16, j * 16) - Main.screenPosition;

            if (!Main.drawToScreen)
                drawOffset += new Vector2(Main.offScreenRange);

            Color drawColor = Lighting.GetColor(i, j);

            drawColor.R *= (byte)(paintColor.R / 255);
            drawColor.G *= (byte)(paintColor.G / 255);
            drawColor.B *= (byte)(paintColor.B / 255);

            spriteBatch.Draw(grassTexture, drawOffset, sourceRectangle, drawColor, 0.0f, Vector2.Zero, 1f, SpriteEffects.None, 0.0f);
        }

        public override void SpecialDraw(int i, int j, SpriteBatch spriteBatch)
        {
            // TO-DO: theres absolutely a better way to do this all lmao
            Texture2D grass = ModContent.Request<Texture2D>("TwilightEgress/Content/Tiles/EnchantedOvergrowth/OvergrowthGrassGrass").Value;
            Color paintColor = WorldGen.paintColor(Main.tile[i, j].TileColor);
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
                DrawExtra(spriteBatch, grass, i + frame.X, j + frame.Y, paintColor, sourceRectangle);
            }
        }
    }
}
