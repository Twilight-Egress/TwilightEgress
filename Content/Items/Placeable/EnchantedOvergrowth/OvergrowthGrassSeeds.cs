using TwilightEgress.Content.Tiles.EnchantedOvergrowth;

namespace TwilightEgress.Content.Items.Placeable.EnchantedOvergrowth
{
    public class OvergrowthGrassSeeds : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.useTime = 10;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.maxStack = 9999;
        }

        public override bool? UseItem(Player player)
        {
            Tile tile = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY);
            Tile tileAbove = Framing.GetTileSafely(Player.tileTargetX, Player.tileTargetY - 1);

            if (!player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple))
                return false;

            if (!tile.HasTile || tile.TileType != ModContent.TileType<Tiles.EnchantedOvergrowth.OvergrowthDirt>())
                return false;

            if (tileAbove.HasTile || tileAbove.LiquidAmount != 0)
                return false;

            Main.tile[Player.tileTargetX, Player.tileTargetY].TileType = (ushort)ModContent.TileType<OvergrowthGrass>();
            SoundEngine.PlaySound(SoundID.Dig, player.Center);

            return true;
        }
    }
}
