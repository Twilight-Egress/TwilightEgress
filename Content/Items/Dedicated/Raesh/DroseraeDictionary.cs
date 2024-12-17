using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Dedicated.Raesh
{
    public class DroseraeDictionary : ModItem
    {
        public override string LocalizationCategory => "Items.Dedicated";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 1;

        public override void SetDefaults()
        {
            Item.width = Item.height = 50;
            Item.damage = 50;
            Item.crit = 4;
            Item.mana = 7;
            Item.knockBack = 6;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.channel = true;
            Item.noMelee = true;
            Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.DamageType = DamageClass.Magic;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.UseSound = SoundID.Item60;
            Item.rare = ItemRarityID.Yellow;
            Item.value = CalamityGlobalItem.RarityYellowBuyPrice;
            Item.shoot = ModContent.ProjectileType<DroseraeDictionaryHoldout>();
            Item.shootSpeed = 1f;
        }
    }
}
