using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwilightEgress.Content.Items.Materials;

namespace TwilightEgress.Content.Items.Weapons.Ranged.SparklingExplosives
{
    public class SparklingGrenade : ModItem
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
            Item.damage = 65;
            Item.DamageType = DamageClass.Ranged;
            Item.knockBack = 8f;
            Item.shootSpeed = 7f;
            Item.shoot = ModContent.ProjectileType<SparklingGrenadeProjectile>();
            Item.width = 20;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.UseSound = SoundID.Item1;
            Item.useAnimation = 40;
            Item.useTime = 40;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.value = Item.sellPrice(copper: 20);
            Item.rare = ItemRarityID.White;
        }

        public override void AddRecipes()
        {
            CreateRecipe(3)
                .AddIngredient<Stargel>()
                .AddIngredient(ItemID.Grenade, 3)
                .Register();
        }
    }
}
