using Terraria.DataStructures;
using Terraria.ID;

namespace TwilightEgress.Content.Items.Materials
{
    public class Cosmostone : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(6, 16));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.consumable = true;
            Item.noMelee = true;
            Item.useAnimation = 10;
            Item.useTime = 10;
            Item.material = true;
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.LightBlue.ToVector3());
            if (Item.Center.Y <= Main.maxTilesY - 750f)
                ItemID.Sets.ItemNoGravity[Type] = true;
            else
                ItemID.Sets.ItemNoGravity[Type] = false;
        }
    }
}
