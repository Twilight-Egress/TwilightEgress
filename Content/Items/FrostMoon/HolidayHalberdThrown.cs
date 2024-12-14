using CalamityMod;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;

namespace TwilightEgress.Content.Items.FrostMoon
{
    public class HolidayHalberdThrown : ModProjectile, IPixelatedPrimitiveRenderer
    {
        private ref float Timer => ref Projectile.ai[0];

        private ref float HalberdPowerScale => ref Projectile.ai[1];

        private Player Owner => Main.player[Projectile.owner];

        public override string LocalizationCategory => "Items.FrostMoon.HolidayHalberd.Projectiles";

        public override string Texture => "TwilightEgress/Assets/Textures/Items/FrostMoon/HolidayHalberd";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 20;
            ProjectileID.Sets.TrailingMode[Type] = 1;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 32;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = Projectile.timeLeft <= 180;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.timeLeft = 240;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 16;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float collisionPoint = 0f;
            float lineLength = 78f * Projectile.scale;
            Vector2 startPoint = Projectile.Center + Projectile.rotation.ToRotationVector2();
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), startPoint, startPoint + Projectile.rotation.ToRotationVector2() * lineLength, 32, ref collisionPoint);
        }

        public override void AI()
        {
            // Projectiles.
            if (Timer % 4 == 0)
            {
                // Spawn two waves of baubles similarly to Berdly's Halberd Attack.
                Vector2 baubleVelocity = Vector2.Normalize(Projectile.velocity).RotatedBy(MathHelper.PiOver2);
                Projectile.BetterNewProjectile(Projectile.Center, baubleVelocity, ModContent.ProjectileType<HolidayHalberdAcceleratingBauble>(), (int)(Projectile.damage * 0.35f), Projectile.knockBack, owner: Projectile.owner);
                Vector2 baubleVelocity2 = Vector2.Normalize(Projectile.velocity).RotatedBy(-MathHelper.PiOver2);
                Projectile.BetterNewProjectile(Projectile.Center, baubleVelocity2, ModContent.ProjectileType<HolidayHalberdAcceleratingBauble>(), (int)(Projectile.damage * 0.35f), Projectile.knockBack, owner: Projectile.owner);
            }

            if (Main.rand.NextBool(3))
            {
                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 75f + Main.rand.NextVector2Circular(35f, 35f);
                float scale = Main.rand.NextFloat(0.2f, 0.8f);
                int lifespan = Main.rand.Next(15, 30);
                SparkleParticle sparkleParticle = new(spawnPosition, Vector2.Zero, GetHalberdVisualColors(), GetHalberdVisualColors() * 0.35f, scale, lifespan, 0.25f, 1.25f);
                sparkleParticle.SpawnCasParticle();
            }

            Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Deltarune reference noway
            if (Projectile.Calamity().stealthStrike && Main.myPlayer == Projectile.owner)
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = target.Center + Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / 3) * 50f;
                    Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<HolidayHalberdIceShock>(), Projectile.damage, Projectile.knockBack, owner: Projectile.owner);
                }
            }
        }

        public override void OnKill(int timeLeft)
        {

            for (int i = 0; i < 12; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(3f, 10f);
                Color initialColor = Color.Lerp(Color.Red, Color.Lime, Main.rand.NextFloat()) * Main.rand.NextFloat(0.2f, 0.75f);
                if (Owner.Calamity().rogueStealth > 0f)
                    initialColor = Color.Lerp(Color.LightSkyBlue, Color.Cyan, Main.rand.NextFloat()) * Main.rand.NextFloat(0.2f, 0.75f);

                Color fadeColor = Color.WhiteSmoke;
                float scale = Main.rand.NextFloat(1f, 3f);
                float opacity = Main.rand.NextFloat(0.8f, 1.75f);
                MediumMistParticle deathSmoke = new(Projectile.Center, velocity, initialColor, fadeColor, scale, opacity, Main.rand.Next(60, 120), 0.03f);
                deathSmoke.SpawnCasParticle();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHalberd();
            return false;
        }

        public Color GetHalberdVisualColors()
        {
            Color mainColor = Utilities.ColorSwap(Color.Red, Color.Lime, 2f);
            Color stealthColor = Utilities.MulticolorLerp(Main.GlobalTimeWrappedHourly / 2f, Color.Cyan, Color.LightCyan, Color.LightSkyBlue, Color.LightBlue);
            if (Owner.Calamity().rogueStealth > 0f)
                mainColor = Color.Lerp(mainColor, stealthColor, Owner.Calamity().rogueStealth / Owner.Calamity().rogueStealthMax);

            return mainColor;
        }

        public void DrawHalberd()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // This is all explained in MSK's file if you're having trouble understanding.
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;
            float baseDrawAngle = Projectile.rotation;
            float drawRotation = baseDrawAngle + MathHelper.PiOver4 + extraAngle;

            Vector2 origin = new Vector2(Owner.direction < 0 ? texture.Width : 0f, texture.Height);
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw backglow effects. 
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 4) * 3f;
                Main.EntitySpriteDraw(texture, backglowDrawPositon, null, Projectile.GetAlpha(GetHalberdVisualColors()), drawRotation, origin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, effects, 0);
        }

        public float TrailWidthFunction(float trailLengthInterploant) => 30f * Utils.GetLerpValue(0f, 1.35f, trailLengthInterploant / 0.1f, true) * Utils.GetLerpValue(1.35f, 0f, (trailLengthInterploant - 0.1f) / 0.9f, true) * Projectile.scale;

        public Color TrailColorFunction(float trailLengthInterploant) => GetHalberdVisualColors();

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {

            ShaderManager.TryGetShader("TwilightEgress.SmoothTextureMapTrail", out ManagedShader smoothTrail);
            smoothTrail.SetTexture(AssetRegistry.Textures.FadedStreak, 1, SamplerState.LinearWrap);
            smoothTrail.TrySetParameter("time", Main.GlobalTimeWrappedHourly);

            Vector2 trailOffset = Projectile.Size * 0.5f + Vector2.UnitX.RotatedBy(Projectile.rotation) * 90f;
            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, _ => trailOffset, true, true, smoothTrail);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 40);
        }
    }
}
