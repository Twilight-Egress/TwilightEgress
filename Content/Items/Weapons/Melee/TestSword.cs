using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Weapons.Melee
{
    public class TestSword : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override string Texture => "Terraria/Images/Item_" + ItemID.FirstFractal;

        public override void SetStaticDefaults() => ItemID.Sets.BonusAttackSpeedMultiplier[Type] = 1f;

        public override void SetDefaults()
        {
            Item.DamageType = DamageClass.Melee;
            Item.damage = 54321;
            Item.useTime = 30;
            Item.crit = 100;
            Item.knockBack = 1;
            Item.useAnimation = 30;
            Item.value = 1;
            Item.rare = ItemRarityID.Expert;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.UseSound = SoundID.LucyTheAxeTalk;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.shootsEveryUse = true;
        }

        public override bool? UseItem(Player player)
        {
            Main.NewText(player.GetWeaponAttackSpeed(Item));
            return base.UseItem(player);
        }
    }
}
