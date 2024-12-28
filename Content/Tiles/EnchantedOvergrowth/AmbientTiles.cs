using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class AmbientTilesLoader : ModSystem
    {
        public override void Load()
        {
            Mod.AddContent(new AmbientTile("AmbientTile1x1", 1, 1, new Point16(0, 0), 4));
            Mod.AddContent(new AmbientTile("AmbientTile1x2", 1, 2, new Point16(0, 1), 2));
            Mod.AddContent(new AmbientTile("AmbientTile2x1", 2, 1, new Point16(1, 0), 1));
            Mod.AddContent(new AmbientTile("AmbientTile2x2", 2, 2, new Point16(1, 1), 3));
            Mod.AddContent(new AmbientTile("AmbientTile2x3", 2, 3, new Point16(1, 2), 1));
            Mod.AddContent(new AmbientTile("AmbientTile2x4", 2, 4, new Point16(1, 3), 3));
            Mod.AddContent(new AmbientTile("AmbientTile3x6", 3, 6, new Point16(1, 5), 1));
        }
    }

    [Autoload(false)]
    public class AmbientTile : ModTile
    {
        public string internalName;
        public string tileTexture;

        public int width;
        public int height;
        public Point16 origin;
        public int randomStyleRange;

        public override string LocalizationCategory => "Tiles.EnchantedOvergrowth";

        public override string Name => internalName;
        public override string Texture => tileTexture;

        public AmbientTile(string internalName, int width, int height, Point16 origin, int randomStyleRange)
        {
            this.internalName = internalName;
            this.tileTexture = "TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/" + internalName;
            this.width = width;
            this.height = height;
            this.origin = origin;
            this.randomStyleRange = randomStyleRange;
        }

        public override void SetStaticDefaults()
        {
            Main.tileLavaDeath[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.Width = width;
            TileObjectData.newTile.Height = height;
            TileObjectData.newTile.StyleHorizontal = true;

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

            HitSound = SoundID.Grass;
            AddMapEntry(new Color(86, 45, 200));
        }
    }
}
