using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class BouncePad : ModTile
    {
        public override string LocalizationCategory => "Tiles.EnchantedOvergrowth";

        public override string Texture => "TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/Manastone";

        public override void SetStaticDefaults()
        {
            TileID.Sets.ChecksForMerge[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileMerge[ModContent.TileType<OvergrowthDirt>()][Type] = true;

            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
            RegisterItemDrop(ModContent.ItemType<ManastoneItem>());
            AddMapEntry(new Color(49, 42, 146));
        }

        public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
            => WorldGen.TileMergeAttempt(-2, ModContent.TileType<OvergrowthDirt>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);

        public override void PostTileFrame(int i, int j, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            float randomForTile = TwilightEgressUtilities.RandomFromVector(new Vector2(i, j));

            if (randomForTile <= 0.12f)
                tile.TileFrameY += 270;
        }
    }

    public class BouncePadItem : ModItem
    {
        public override string LocalizationCategory => "Tiles.EnchantedOvergrowth.BouncePad.Items";

        public override string Texture => "TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/Manastone_Item";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BouncePad>());
            Item.width = 16;
            Item.height = 16;
        }
    }

    public class BouncePlayer : ModPlayer
    {
        public override void PreUpdateMovement()
        {
            foreach (Point touchedTile in Player.TouchedTiles)
            {
                Tile tile = Main.tile[touchedTile.X, touchedTile.Y];
                if (tile != null && tile.HasTile && tile.HasUnactuatedTile && tile.TileType == ModContent.TileType<BouncePad>())
                {
                    if (Player.velocity.Y > 0)
                    {
                        Player.velocity.Y *= -1f;
                        Player.velocity.Y -= 5;

                        Player.velocity.X *= 2f;
                    }

                    break;
                }
            }

            base.PreUpdateMovement();
        }
    }
}
