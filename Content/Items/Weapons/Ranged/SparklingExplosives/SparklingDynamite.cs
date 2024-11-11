using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilightEgress.Content.Items.Materials;

namespace TwilightEgress.Content.Items.Weapons.Ranged.SparklingExplosives
{
    public class SparklingDynamite : ModItem
    {
        public new string LocalizationCategory => "Items.Weapons.Ranged";

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatCountAsBombsForDemolitionistToSpawn[Type] = true;
            Item.ResearchUnlockCount = 99;
        }

        public override void SetDefaults()
        {
            Item.useStyle = ItemUseStyleID.Swing;
            Item.shootSpeed = 12f;
            Item.shoot = ProjectileID.Dynamite;
            Item.width = 14;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(silver: 4);
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Stargel>()
                .AddIngredient(ItemID.Dynamite)
                .Register();
        }
    }
}
