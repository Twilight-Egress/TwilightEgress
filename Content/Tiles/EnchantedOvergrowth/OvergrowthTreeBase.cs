using CalamityMod.NPCs.TownNPCs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Enums;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    // to-do: use tilestyles instead of 3 different base types

    public class OvergrowthTreeBase1 : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

            Main.tileAxe[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.Width = 7;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(3, 3);

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.RandomStyleRange = 4;

            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            RegisterItemDrop(ItemID.Wood);
            AddMapEntry(new Color(80, 55, 74));
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

    public class OvergrowthTreeBase2 : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

            Main.tileAxe[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.Width = 7;
            TileObjectData.newTile.Height = 4;

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(2, 3);

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.RandomStyleRange = 2;

            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            RegisterItemDrop(ItemID.Wood);
            AddMapEntry(new Color(80, 55, 74));
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

    public class OvergrowthTreeBase3 : ModTile
    {
        public override void SetStaticDefaults()
        {
            TileID.Sets.PreventsTileRemovalIfOnTopOfIt[Type] = true;
            TileID.Sets.PreventsTileReplaceIfOnTopOfIt[Type] = true;

            Main.tileAxe[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = false;
            Main.tileSolid[Type] = false;

            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;

            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(2, 2);

            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.RandomStyleRange = 4;

            TileObjectData.addTile(Type);

            HitSound = SoundID.Dig;
            RegisterItemDrop(ItemID.Wood);
            AddMapEntry(new Color(80, 55, 74));
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
