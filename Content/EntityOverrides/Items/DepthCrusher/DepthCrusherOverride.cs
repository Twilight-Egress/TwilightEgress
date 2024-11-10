namespace TwilightEgress.Content.EntityOverrides.Items.DepthCrusher
{
    public class DepthCrusherOverride : ItemOverride
    {
        public override int TypeToOverride => ModContent.ItemType<CalamityMod.Items.Weapons.Melee.DepthCrusher>();

        public override void SetDefaults(Item item)
	    {
            item.damage = 70;
            item.useTime = 50;
            item.useAnimation = 50;
            item.knockBack = 10f;
            item.hammer = 0;
            item.DamageType = DamageClass.MeleeNoSpeed;
            item.noUseGraphic = true;
            item.useTurn = false;
            item.useStyle = ItemUseStyleID.Swing;
        }

        public override bool AltFunctionUse(Item item, Player player) => true;

        public override bool? UseItem(Item item, Player player)
        {
            if (player.altFunctionUse == 2)
            {
                item.hammer = 70;
                item.noUseGraphic = false;
                item.shoot = ProjectileID.None;
                item.useTurn = true;
            }
            else
            {
                item.hammer = 0;
                item.noUseGraphic = true;
                item.useTurn = false;
            }

            return true;
        }
    }
}