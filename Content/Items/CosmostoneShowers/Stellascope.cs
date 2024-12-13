using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Items.CosmostoneShowers
{
    public class Stellascope : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 25;
            Item.height = 8;
            Item.DamageType = DamageClass.Magic;
            Item.damage = 24;
            Item.useTime = Item.useAnimation = 70;
            Item.knockBack = 0;
            Item.mana = 12;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.shoot = ModContent.ProjectileType<StellascopeHoldout>();
            Item.channel = true;
            Item.value = 12504;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.rare = ItemRarityID.Green;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, Item.shoot, damage, knockback, player.whoAmI);
            return false;
        }

        public override bool CanUseItem(Player player) => player.ownedProjectileCounts[Item.shoot] <= 0;
    }

    public class StellascopeHoldout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private const int ManaCost = 12;

        public override string Texture => ModContent.GetModItem(ModContent.ItemType<Stellascope>()).Texture + "_Holdout";

        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity = Vector2.Normalize(Main.MouseWorld - Owner.MountedCenter);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.direction == 1 ? 0f : MathHelper.Pi);
            Projectile.Center = Owner.Center;
            UpdatePlayerVariables();

            if (Timer >= 70 && Main.myPlayer == Projectile.owner && Owner.CheckMana(ManaCost, true))
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Main.MouseWorld, Vector2.Zero, ModContent.ProjectileType<StellascopeStar>(), Projectile.damage, Projectile.knockBack);
                Timer = 0;
            }

            if (Owner.CantUseHoldout())
                Projectile.Kill();
        }

        private void UpdatePlayerVariables()
        {
            Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, -MathHelper.Pi / 1.5f * Projectile.direction);
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Asset<Texture2D> holdoutTexture = TextureAssets.Projectile[Type];
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(10 * Projectile.direction, -8);
            Vector2 rotationOrigin = holdoutTexture.Size() * 0.5f - new Vector2(20 * Projectile.direction, 0);
            SpriteEffects spriteFlip = Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Main.EntitySpriteDraw(holdoutTexture.Value, drawPosition, holdoutTexture.Frame(), lightColor, Projectile.rotation, rotationOrigin, 1f, spriteFlip);
            return false;
        }

        public override bool ShouldUpdatePosition() => false;

        public override bool? CanDamage() => false;
    }
}
