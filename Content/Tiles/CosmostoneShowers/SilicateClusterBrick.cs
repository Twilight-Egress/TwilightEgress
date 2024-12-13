using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using TwilightEgress.Content.Items.CosmostoneShowers;

namespace TwilightEgress.Content.Tiles.CosmostoneShowers
{
    public class SilicateClusterBrick : ModTile
    {
        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            MineResist = 1.5f;
            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(82, 95, 192));
        }
    }

    public class SilicateClusterBrickItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";

        public override string Texture => ModContent.GetModTile(ModContent.TileType<SilicateClusterBrick>()).Texture.Replace("Content", "Assets/Textures") + "_Item";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilicateClusterBrick>());
            Item.width = 12;
            Item.height = 12;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<SilicateCluster>(2)
                .AddTile(TileID.Furnaces)
                .Register();
        }
    }
}
