using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class Stargel : ModItem
    {
        public new string LocalizationCategory => "Items.Materials";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 5;

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;

            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 2);
        }
    }
}
