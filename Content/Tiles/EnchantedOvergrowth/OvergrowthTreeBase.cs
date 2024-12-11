using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.Enums;
using Terraria.GameContent.Drawing;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    // to-do: add 3rd large type

    public class OvergrowthTreeBaseLoader : ModSystem
    {
        public override void Load()
        {
            Mod.AddContent(new OvergrowthTreeBase("OvergrowthTreeBaseLarge", 7, 4, new Point16(3, 3), 4));
            Mod.AddContent(new OvergrowthTreeBase("OvergrowthTreeBaseLarge2", 7, 4, new Point16(2, 3), 2));
            Mod.AddContent(new OvergrowthTreeBase("OvergrowthTreeBaseMedium", 5, 3, new Point16(2, 2), 4));
            Mod.AddContent(new OvergrowthTreeBase("OvergrowthTreeBaseSmall", 3, 2, new Point16(1, 1), 4, false));
        }
    }

    [Autoload(false)]
    public class OvergrowthTreeBase : ModTile
    {
        public string internalName;
        public string tileTexture;

        public int width;
        public int height;
        public Point16 origin;
        public int randomStyleRange;
        public bool hasGlow;

        private Asset<Texture2D> glowTexture;

        public override string Name => internalName;
        public override string Texture => tileTexture;

        public OvergrowthTreeBase(string internalName, int width, int height, Point16 origin, int randomStyleRange, bool hasGlow = true)
        {
            this.internalName = internalName;
            this.tileTexture = "TwilightEgress/Content/Tiles/EnchantedOvergrowth/" + internalName;
            this.width = width;
            this.height = height;
            this.origin = origin;
            this.randomStyleRange = randomStyleRange;
            this.hasGlow = hasGlow;
        }

        public override void SetStaticDefaults()
        {
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

            Main.tileAxe[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;

            TileObjectData.newTile.CoordinateWidth = 16;

            List<int> coordinateHeights = new List<int>();

            for (int i = 0; i < height; i++)
            {
                coordinateHeights.Add(16);
            }

            TileObjectData.newTile.CoordinateHeights = coordinateHeights.ToArray();
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = origin;

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.RandomStyleRange = randomStyleRange;

            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            RegisterItemDrop(ItemID.Wood);
            AddMapEntry(new Color(80, 55, 74));

            if (hasGlow)
                glowTexture = ModContent.Request<Texture2D>(tileTexture + "_Glow");
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile) || !hasGlow)
                return;

            Rectangle sourceRectangle = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
            Color paintColor = WorldGen.paintColor(Framing.GetTileSafely(i, j).TileColor);
            spriteBatch.DrawTileTexture(glowTexture.Value, i, j, sourceRectangle, paintColor, 0f, Vector2.Zero, lighted: false);
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;

            bool up = Framing.GetTileSafely(i, j - 1).TileType == ModContent.TileType<OvergrowthTree>();

            if (up)
                WorldGen.KillTile(i, j - 1);
        }
    }
}
