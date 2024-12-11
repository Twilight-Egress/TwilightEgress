using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Projectiles.Ranged.Ammo;

namespace TwilightEgress.Content.Items.Ammo
{
    public class StingerRound : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Ammo";

        public override void SetStaticDefaults() => Item.ResearchUnlockCount = 99;

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 14;
            Item.damage = 7;
            Item.ArmorPenetration = 10;
            Item.knockBack = 1f;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<StingerRoundProjectile>();
            Item.shootSpeed = 1f;
            Item.ammo = AmmoID.Bullet;
        }
    }
}
