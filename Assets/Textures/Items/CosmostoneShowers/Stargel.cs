namespace TwilightEgress.Content.Items.Materials
{
    public class Stargel : ModItem
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 5;

        public override void SetDefaults()
        {
            Item.width = Item.height = 16;

            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 2);
        }
    }
}
