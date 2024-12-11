using CalamityMod;
using CalamityMod.Items;
using CalamityMod.Rarities;
using Terraria.DataStructures;
using TwilightEgress.Core.Players;

namespace TwilightEgress.Content.Items.Weapons.Melee.ResplendentRoar
{
    public class ResplendentRoar : ModItem, ILocalizedModType
    {
        public int SwingDirection { get; set; }

        public static int AttackCounter { get; set; }

        public new string LocalizationCategory => "Items.Weapons.Melee";

        public override string Texture => "CalamityMod/Items/Weapons/Melee/TheBurningSky";

        public override void SetStaticDefaults() => ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;

        public override void SetDefaults()
        {
            Item.width = 102;
            Item.height = 146;
            Item.damage = 200;
            Item.knockBack = 3f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.DamageType = ModContent.GetInstance<TrueMeleeDamageClass>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.value = CalamityGlobalItem.RarityVioletBuyPrice;
            Item.rare = ModContent.RarityType<Violet>();
            Item.shoot = ModContent.ProjectileType<ResplendentRoarHoldout>();
            Item.shootSpeed = 1f;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] < 1;

        public override bool AltFunctionUse(Player player) => player.GetModPlayer<ResplendentRoarPlayer>().ResplendentRazeCharge >= 10f;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            // Reset the resplendent raze update timer.
            player.GetModPlayer<ResplendentRoarPlayer>().ResplendentRazeUpdateTimer = 0;

            AttackCounter++;
            if (AttackCounter >= 4)
                AttackCounter = 0;

            if (SwingDirection is not -1 and not 1)
                SwingDirection = 1;
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: AttackCounter, ai2: SwingDirection);
            if (Main.projectile.IndexInRange(p))
            {
                if (player.altFunctionUse == 2)
                    Main.projectile[p].localAI[0] = 1f;
            }

            SwingDirection *= -1;
            return false;
        }
    }
}
