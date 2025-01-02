using CalamityMod.Items;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress.Content.Items.Dedicated.MPG
{
    public class MoonSpiritKhakkhara : ModItem
    {
        public override string LocalizationCategory => "Items.Dedicated";

        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 64;
            Item.mana = 25;
            Item.damage = 310;
            Item.knockBack = 3f;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.noMelee = true;
            Item.channel = true;
            Item.noUseGraphic = true;
            Item.autoReuse = true;
            Item.UseSound = SoundID.Item105;
            Item.DamageType = DamageClass.Summon;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            Item.rare = ModContent.RarityType<DarkBlue>();
            Item.shootSpeed = 0f;
            Item.shoot = ModContent.ProjectileType<MoonSpiritKhakkharaHoldout>();
        }

        public override bool AltFunctionUse(Player player) => player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] > 0;

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float attackType = 0f;
            if (player.altFunctionUse == 2)
                attackType = 1f;

            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai1: attackType);
            return false;
        }

        public override bool? UseItem(Player player)
        {
            // Get a list of all active Underworld Lanterns.
            List<Projectile> lanterns = new List<Projectile>();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.active && p.type == ModContent.ProjectileType<UnderworldLantern>())
                    lanterns.Add(p);
            }

            Projectile lantern = lanterns.FirstOrDefault();
            if (player.altFunctionUse == 2 && player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] > 0)
            {
                SpawnSkulls(player);
                player.GetModPlayer<BuffHandler>().CurseOfNecromancyMinionSlotStack++;
                player.AddBuff(ModContent.BuffType<CurseOfNecromancy>(), 3600);
                lantern.Kill();
            }

            return true;
        }

        public void SpawnSkulls(Player player)
        {
            int type = ModContent.ProjectileType<CurseOfNecromancySkull>();
            int p = Projectile.NewProjectile(new EntitySource_WorldEvent(), player.Center, Vector2.Zero, type, 0, 0f, player.whoAmI);
            if (Main.projectile.IndexInRange(p))
                (Main.projectile[p].ModProjectile as CurseOfNecromancySkull).SkullIndex = player.ownedProjectileCounts[type];
            SoundEngine.PlaySound(SoundID.DD2_BetsysWrathShot, player.Center);

            int skullIndex = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                if (Main.projectile[i].type == type && Main.projectile[i].active && Main.projectile[i].owner == player.whoAmI)
                {
                    (Main.projectile[p].ModProjectile as CurseOfNecromancySkull).SkullIndex = skullIndex++;
                    (Main.projectile[p].ModProjectile as CurseOfNecromancySkull).Timer = 0f;
                    Main.projectile[i].netUpdate = true;
                }
            }
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<StaffOfNecrosteocytes>())
                .AddIngredient(ModContent.ItemType<ClothiersWrath>())
                .AddIngredient(ModContent.ItemType<NightmareFuel>(), 20)
                .AddTile(ModContent.TileType<CosmicAnvil>())
                .Register();
        }
    }

    public class MoonSpiritKhakkharaUseFix : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            if (item.DamageType == DamageClass.Summon && player.HeldItem.type != ModContent.ItemType<MoonSpiritKhakkhara>())
                return player.ownedProjectileCounts[ModContent.ProjectileType<UnderworldLantern>()] < 1;
            return base.CanUseItem(item, player);
        }
    }
}
