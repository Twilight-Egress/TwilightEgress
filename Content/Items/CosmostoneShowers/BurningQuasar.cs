using CalamityMod.Items;
using CalamityMod.Items.Weapons.Ranged;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class BurningQuasar : ModItem
    {
        public override string LocalizationCategory => "Items.CosmostoneShowers";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }
        public override void SetDefaults()
        {
            Item.width = 66;
            Item.height = 24;
            Item.damage = 20;
            Item.knockBack = 0.425f;
            Item.useTime = 6;
            Item.useAnimation = 30;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = 5;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.shootSpeed = 8.5f;
            Item.useAmmo = AmmoID.Gel;
            Item.shoot = ProjectileID.Flames; //why is it red tho i stole this from elf melter not flamethrower wtf..,.,.,.
                                              // Top ten reasons to not use Visual Studio Code iirc -fryzahh
            Item.UseSound = SoundID.Item34;
            Item.consumeAmmoOnFirstShotOnly = true;

        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CosmostoneBar>(), 8)
                .AddIngredient<SparkSpreader>()
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool AltFunctionUse(Player player) => true;

    }
}