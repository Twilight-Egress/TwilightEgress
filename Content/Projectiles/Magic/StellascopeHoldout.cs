using CalamityMod;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Projectiles.Magic
{
    public class StellascopeHoldout : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private const int ManaCost = 12;

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

            if (CalamityUtils.CantUseHoldout(Owner))
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
