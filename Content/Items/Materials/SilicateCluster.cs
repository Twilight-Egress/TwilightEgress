namespace TwilightEgress.Content.Items.Materials
{
    public class SilicateCluster : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 100;

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noMelee = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.material = true;
        }
    }
}
