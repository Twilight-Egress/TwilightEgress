using CalamityMod;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.Jungle
{
    public class PoisonArrow : ModItem
    {
        public override string LocalizationCategory => "Items.Jungle";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 32;
            Item.damage = 11;
            Item.knockBack = 2;
            Item.DamageType = DamageClass.Ranged;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.value = Item.sellPrice(copper: 1);
            Item.rare = ItemRarityID.Blue;
            Item.shoot = ModContent.ProjectileType<PoisonArrowProjectile>();
            Item.shootSpeed = 5f;
            Item.ammo = AmmoID.Arrow;
        }

        public override void AddRecipes()
        {
            CreateRecipe(50)
                .AddIngredient(ItemID.WoodenArrow, 50)
                .AddIngredient(ItemID.Stinger, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }

    public class PoisonArrowProjectile : ModProjectile
    {
        public override string LocalizationCategory => "Items.Jungle.PoisonArrow.Projectiles";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.WoodenArrowFriendly;
            Projectile.arrow = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.Calamity().pointBlankShotDuration = 18;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int chance = hit.Crit ? 1 : 4;
            if (Main.rand.NextBool(chance))
            {
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustRotation = Vector2.Normalize(Vector2.UnitY).RotatedBy(i - (15 / 2 - 1) * MathHelper.TwoPi / 15) + Projectile.Center;
                    Vector2 dustVelocity = dustRotation - Projectile.Center;
                    Dust dust = Dust.NewDustPerfect(dustRotation + dustVelocity, 18, Vector2.Normalize(dustVelocity) * 6f);
                    dust.noGravity = true;
                }
                target.AddBuff(BuffID.Poisoned, 180);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}
