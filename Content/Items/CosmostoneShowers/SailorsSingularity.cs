using CalamityMod.Items;
using Terraria;
using Terraria.ID;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using CalamityMod;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.GameContent;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class SailorsSingularity : ModItem
    {
        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

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
            Item.channel = true;
            Item.knockBack = 0.425f;
            Item.useTime = Item.useAnimation = 20;
            Item.noMelee = true;
            Item.autoReuse = false;
            Item.DamageType = DamageClass.Ranged;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.value = CalamityGlobalItem.RarityBlueBuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.noUseGraphic = true;
            Item.shoot = ModContent.ProjectileType<SailorsSingularityHoldout>();
            Item.shootSpeed = 11f;
        }
        public override bool CanUseItem(Player player)
        {
            return !(player.altFunctionUse == 2 && Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<SailorsSingularityVortex>()));
        }
        public override bool CanShoot(Player player) => true;
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) => true;
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<CosmostoneBar>(), 8)
                .AddIngredient(ItemID.StarCannon)
                .AddTile(TileID.Anvils)
                .Register();
        }

        public override bool AltFunctionUse(Player player) => true;
    }

    public class SailorsSingularityHoldout : ModProjectile
    {
        public override string Texture => "TwilightEgress/Assets/Textures/Items/CosmostoneShowers/SailorsSingularity";
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
        }
        private int attackCounter = 0;
        private int counter = 0;
        private int despawnCounter;
        Vector2 toMouse = Vector2.Zero;
        float fireRotation = 0f;
        private static bool swingUp = true;
        private void UpdateProjectileHeldVariables()
        {
            Projectile.velocity = Vector2.Zero;
            Player owner = Main.player[Projectile.owner];
            Projectile.Center = owner.Center;
            Projectile.rotation = fireRotation;
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
        }

        private void ManipulatePlayerVariables()
        {
            Player owner = Main.player[Projectile.owner];
            owner.ChangeDir((int)(Projectile.rotation.ToRotationVector2().X / Math.Abs(Projectile.rotation.ToRotationVector2().X)));
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, fireRotation - MathHelper.PiOver2);
        }
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];
            if (!owner.active || owner.dead || !owner.channel && despawnCounter >= 30 || (!owner.Calamity().mouseRight || despawnCounter >= 30) && owner.altFunctionUse == 2)
            {
                attackCounter = 0;
                counter = 0;
                Projectile.active = false;
            }
            else
            {
                if (owner.altFunctionUse != 2)
                {
                    if (!owner.channel)
                        despawnCounter++;
                    else
                        despawnCounter = 0;

                    if (despawnCounter != 0)
                    {
                        if (despawnCounter == 1)
                        {
                            counter = 0;
                            swingUp = swingUp ? false : true;
                        }
                        float range = MathHelper.Pi / 2 - (float)(attackCounter / 8f);
                        Vector2 position = owner.Center - Vector2.UnitY * 24 + toMouse * 20;
                        int rotationDirection = swingUp ? -1 : 1;

                        if (counter == 0)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
                            toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                        }
                        if (attackCounter != 14) //Uncharged Attack
                        {
                            if (counter < 10 && Main.myPlayer == owner.whoAmI)
                            {
                                fireRotation = toMouse.RotatedBy(range / 2 * rotationDirection + range / 10 * counter * rotationDirection - range * rotationDirection).ToRotation();
                                if (counter % 2 == 0 && Main.myPlayer == Projectile.owner)
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, toMouse.RotatedBy(range / 2 * rotationDirection + range / 4 * (counter / 2) * rotationDirection - range * rotationDirection) * 10f, ModContent.ProjectileType<SailorsSingularityStar>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                            }
                        }
                        else //Max Charge Attack
                        {
                            if (counter == 0 && Main.myPlayer == owner.whoAmI)
                            {
                                fireRotation = toMouse.ToRotation();
                                if (Main.myPlayer == Projectile.owner)
                                {
                                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), position, toMouse * 12f, ModContent.ProjectileType<SailorsSingularityBlast>(), Projectile.damage * 3, Projectile.knockBack, owner.whoAmI);
                                    owner.velocity += toMouse.RotatedBy(MathHelper.Pi) * 5f;
                                }
                            }
                            if (despawnCounter < 15)
                                despawnCounter = 15;
                            fireRotation = fireRotation.ToRotationVector2().RotatedBy(0.41f / (despawnCounter - 14) * -owner.direction).ToRotation();
                        }
                        counter++;
                    }
                    else
                    {
                        toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                        fireRotation = toMouse.ToRotation();
                        if (attackCounter == 14)
                            fireRotation = fireRotation.ToRotationVector2().RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)).ToRotation();
                        if (counter % 4 == 0 && attackCounter < 14)
                            attackCounter++;
                        counter++;
                    }
                }
                else
                {
                    if (!owner.Calamity().mouseRight || Main.projectile.Any(p => p.active && p.type == ModContent.ProjectileType<SailorsSingularityVortex>()))
                        despawnCounter++;
                    else
                        despawnCounter = 0;
                    toMouse = (Main.MouseWorld - owner.Center).SafeNormalize(Vector2.Zero);
                    if (counter < 45)
                        fireRotation = toMouse.ToRotation();
                    if (counter >= 45 && Main.myPlayer == owner.whoAmI)
                    {
                        if (counter == 45)
                        {
                            SoundEngine.PlaySound(SoundID.DD2_BookStaffCast);
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), new Vector2(Projectile.Center.X, Projectile.Center.Y), toMouse * 5f, ModContent.ProjectileType<SailorsSingularityVortex>(), Projectile.damage, Projectile.knockBack, owner.whoAmI);
                        }
                        if (despawnCounter < 15)
                            despawnCounter = 15;
                        fireRotation = fireRotation.ToRotationVector2().RotatedBy(0.41f / (despawnCounter - 14) * -owner.direction).ToRotation();
                    }
                    counter++;
                }
                UpdateProjectileHeldVariables();
                ManipulatePlayerVariables();
            }
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 origin = TextureAssets.Projectile[Projectile.type].Size() * 0.5f + new Vector2();
            Vector2 drawPosition = Projectile.Center + Vector2.UnitY * (Projectile.gfxOffY - 8) - Main.screenPosition;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (Main.player[Projectile.owner].direction == -1)
                spriteEffects = SpriteEffects.FlipVertically;
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), Projectile.rotation, origin - new Vector2(24, -4), Projectile.scale, spriteEffects, 0f);
            return false;
        }
    }
}
