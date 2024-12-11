using CalamityMod.Items;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Content.Buffs.Minions;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Items.Accessories.Elementals.TwinGeminiGenies
{
    public class GeminiGenieLamp : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";

        public override string Texture => "Terraria/Images/Item_" + ItemID.SpiritFlame;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 18;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.RarityPinkBuyPrice;
            Item.rare = ItemRarityID.Pink;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.FindBuffIndex(ModContent.BuffType<GeminiGenies>()) == -1)
                player.AddBuff(ModContent.BuffType<GeminiGenies>(), 2);

            int damage = (int)player.GetTotalDamage<SummonDamageClass>().ApplyTo(75);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGenieSandy>()] < 1)
            {
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGenieSandy>(), damage, 4f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = damage;
            }

            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGeniePsychic>()] < 1)
            {
                int p = Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGeniePsychic>(), damage, 4f, player.whoAmI);
                if (Main.projectile.IndexInRange(p))
                    Main.projectile[p].originalDamage = damage;
            }
        }

        public override void UpdateVanity(Player player)
        {
            player.GetModPlayer<BuffHandler>().GeminiGeniesVanity = true;
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGenieSandy>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGenieSandy>(), 0, 0f, player.whoAmI);
            if (player.ownedProjectileCounts[ModContent.ProjectileType<GeminiGeniePsychic>()] < 1)
                Projectile.NewProjectile(player.GetSource_Accessory(Item), player.Center, Vector2.Zero, ModContent.ProjectileType<GeminiGeniePsychic>(), 0, 0f, player.whoAmI);
        }
    }
}
