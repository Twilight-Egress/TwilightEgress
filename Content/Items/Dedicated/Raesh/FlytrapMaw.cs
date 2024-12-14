using CalamityMod;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Core;

namespace TwilightEgress.Content.Items.Dedicated.Raesh
{
    public class FlytrapMaw : ModProjectile, ILocalizedModType
    {
        private ref float ViableTargetIndex => ref Projectile.ai[0];

        private Player Owner => Main.player[Projectile.owner];

        private List<NPC> NPCsWhoHaveBeenHit { get; set; }

        private Asset<Texture2D> trailTexture;

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;

            trailTexture = ModContent.Request<Texture2D>("TwilightEgress/Assets/Textures/Items/Dedicated/Raesh/FlytrapMaw_Chain");
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 28;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 30;
            Projectile.Opacity = 0f;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 5;
        }

        public override void OnSpawn(IEntitySource source)
        {
            // Scale up depending on the player's current life.
            NPCsWhoHaveBeenHit = new();
            float scaleFactor = MathHelper.Lerp(1f, 1.75f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
            Projectile.scale = 1f * scaleFactor;
        }

        public override void AI()
        {
            NPC viableTarget = Main.npc[(int)ViableTargetIndex];
            if (viableTarget == null)
            {
                Projectile.Kill();
                return;
            }

            Projectile.Opacity = MathHelper.Clamp(Projectile.Opacity + 0.05f, 0f, 1f);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            Projectile.UpdateProjectileAnimationFrames(0, 4, 5);
            Projectile.AdjustProjectileHitboxByScale(28f, 28f);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (!NPCsWhoHaveBeenHit.Contains(target))
            {
                /* Add the whoAmI indexes of each hit NPC to the list. 
                 * This is how we'll keep track of which NPCs we shouldn't target. */
                NPCsWhoHaveBeenHit.Add(target);
                SoundEngine.PlaySound(AssetRegistry.Sounds.FlytrapMawBounce with { MaxInstances = 1 }, Projectile.Center);
            }

            // Find the closest target in range and bounce to them from the last enemy.
            // If there are no targets, carry on as usual.
            NPC viableBounceTarget = Projectile.FindTargetWithinRange(1000f);
            if (viableBounceTarget == null)
                return;

            if (viableBounceTarget.CanBeChasedBy() && !NPCsWhoHaveBeenHit.Contains(viableBounceTarget))
            {
                Projectile.velocity = Projectile.SafeDirectionTo(viableBounceTarget.Center) * 35f;
                ViableTargetIndex = viableBounceTarget.whoAmI;
            }
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1);

            int dustCount = 15 * (int)MathHelper.Lerp(1f, 2f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
            float speed = MathHelper.Lerp(5f, 10f, Utils.GetLerpValue(Owner.statLifeMax, 100f, Owner.statLife, true));
            float scale = Main.rand.NextFloat(0.65f, 1.25f) * Projectile.scale;

            for (int i = 0; i < dustCount; i++)
            {
                Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.Plantera_Green, dustVelocity * speed, Scale: scale);
                dust.noGravity = true;
            }

            for (int i = 0; i < 2; i++)
            {
                int goreType = 388 + i;
                Vector2 goreSpawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.height);
                Gore.NewGore(Projectile.GetSource_Death(), goreSpawnPosition, Projectile.velocity * 0.1f, goreType, Projectile.scale);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                Projectile.Kill();
                return true;
            }
            else
            {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(AssetRegistry.Sounds.FlytrapMawBounce with { MaxInstances = 1 }, Projectile.Center);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon)
                    Projectile.velocity.X = -oldVelocity.X;

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon)
                    Projectile.velocity.Y = -oldVelocity.Y;
            }

            return false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawPrims();
            Projectile.DrawTextureOnProjectile(Color.White, Projectile.rotation, Projectile.scale, animated: true);
            return false;
        }

        public float TrailWidthFunction(float trailLengthInterpolant) => 12f * Utils.GetLerpValue(1f, 0f, trailLengthInterpolant, true) * Projectile.scale * Projectile.Opacity;

        public Color TrailColorFunction(float trailLengthInterpolant) => Color.White * Projectile.Opacity;

        public void DrawPrims()
        {
            Main.spriteBatch.EnterShaderRegion();
            ShaderManager.TryGetShader("TwilightEgress.PrimitiveTextureMapTrail", out ManagedShader textureMapTrailShader);
            textureMapTrailShader.SetTexture(trailTexture, 1);
            textureMapTrailShader.TrySetParameter("mapTextureSize", trailTexture.Size());
            textureMapTrailShader.TrySetParameter("textureScaleFactor", 600f);
            textureMapTrailShader.Apply();

            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, _ => Projectile.Size * 0.5f, true, Shader: textureMapTrailShader);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 48);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
