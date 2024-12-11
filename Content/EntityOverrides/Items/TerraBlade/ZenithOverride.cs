using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.EntityOverrides.Items.TerraBlade
{
    public class ZenithOverride : ItemOverride
    {
        public override int TypeToOverride => ItemID.Zenith;

        public override void AddRecipes()
        {
            Recipe.Create(TypeToOverride)
                .AddIngredient(ItemID.FirstFractal)
                .AddIngredient(ItemID.Meowmere)
                .AddIngredient(ItemID.StarWrath)
                .AddIngredient(ItemID.InfluxWaver)
                .AddIngredient(ItemID.TheHorsemansBlade)
                .AddIngredient(ItemID.Seedler)
                .AddIngredient(ItemID.Starfury)
                .AddIngredient(ItemID.BeeKeeper)
                .AddIngredient(ItemID.EnchantedSword)
                .AddIngredient(ItemID.CopperShortsword)
                .AddIngredient(ModContent.ItemType<AuricBar>(),5)
                .AddTile(TileID.MythrilAnvil)
                .Register();

        }
    }
}