﻿using Luminance.Common.Utilities;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace TwilightEgress.Content.Projectiles.Ranged.SparklingExplosives
{
    public class SparklingGrenadeProjectile : ModProjectile
    {
        public override string Texture => "TwilightEgress/Content/Items/Weapons/Ranged/SparklingExplosives/SparklingGrenade";

        private List<Vector2> mousePositions = new List<Vector2>();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
            ProjectileID.Sets.Explosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.damage = 65;
            Projectile.knockBack = 8f;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 180;

            DrawOffsetX = -1;
            DrawOriginOffsetY = -4;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.timeLeft = 0;
            Projectile.PrepareBombToBlow();
            return true;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                Projectile.timeLeft = 0;
                Projectile.PrepareBombToBlow();
                return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }

        public override void AI()
        {
            Projectile.rotation += 0.05f;
            Lighting.AddLight(Projectile.Center, Color.Blue.ToVector3() * 0.5f);

            if (Projectile.owner != Main.myPlayer)
                return;

            if (Projectile.timeLeft <= 3)
                Projectile.PrepareBombToBlow();
            else {
                Projectile.netUpdate = true;

                if (mousePositions.Count == 0 || ((Main.MouseScreen + Main.screenPosition) - mousePositions.Last()).LengthSquared() >= Pow(16f * 8f, 2))
                    mousePositions.Add(Main.MouseScreen + Main.screenPosition);

                if ((mousePositions.First() - Projectile.Center).LengthSquared() < Pow(16f * 4f, 2))
                    mousePositions.RemoveAt(0);

                List<Vector2> positionsToGoTo = new List<Vector2>();
                positionsToGoTo.AddRange(mousePositions);
                positionsToGoTo.Add(Main.MouseScreen + Main.screenPosition);

                Vector2 toFirstPos = positionsToGoTo.First() - Projectile.Center;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, toFirstPos.SafeNormalize(Vector2.Zero) * 15f, 0.1f);

                if ((positionsToGoTo.First() - Projectile.Center).LengthSquared() < Pow(16f * 1f, 2))
                    Projectile.timeLeft = 3;
            }

            // Rotation increased by velocity
            Projectile.rotation += 0.05f;
        }

        public override void PrepareBombToBlow()
        {
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.Resize(60, 60);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);

            // spawn sparkle particles
            for (int i = 0; i < 10; i++)
            {
                new SparkleParticle(Projectile.Center, Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(6), Color.LightBlue, Color.DarkBlue, Main.rand.NextFloat(0.2f, 0.5f), 20, bloomScale: 0f).SpawnCasParticle();
            }

            Lighting.AddLight(Projectile.Center, Color.LightBlue.ToVector3());

            Projectile.Resize(15, 15);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Texture2D glowTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Projectiles/Ranged/SparklingExplosives/SparklingGrenadeProjectile_Glow", AssetRequestMode.ImmediateLoad).Value;
            AtlasTexture bloomTexture = AtlasManager.GetTexture("TwilightEgress.BloomFlare.png");

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, BlendState.Additive, Main.DefaultSamplerState, default, RasterizerState.CullNone, default);

            Main.spriteBatch.Draw(bloomTexture, Projectile.Center - Main.screenPosition, null, Color.Blue, Projectile.rotation * 1.5f, null, scale: glow.Size() * 0.002f);
            Main.spriteBatch.Draw(glowTexture, Projectile.Center - Main.screenPosition, null, Color.White * 0.7f, Projectile.rotation, glow.Size() * 0.5f, Projectile.scale * 1.1f, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(default, default, Main.DefaultSamplerState, default, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);

            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale, 0, 0);
            return false;
        }
    }
}
