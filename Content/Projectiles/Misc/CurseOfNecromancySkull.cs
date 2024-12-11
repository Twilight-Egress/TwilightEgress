using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria.GameContent;
using Terraria.ID;
using TwilightEgress.Content.Buffs.Debuffs;
using TwilightEgress.Content.Particles;

namespace TwilightEgress.Content.Projectiles.Misc
{
    public class CurseOfNecromancySkull : ModProjectile, ILocalizedModType
    {
        public int SkullIndex;

        public Player Owner => Main.player[Projectile.owner];

        public ref float Timer => ref Projectile.ai[0];

        public float SkullHoverOffsetAngle
        {
            get
            {
                float projectileCounts = Owner.ownedProjectileCounts[Type];
                if (projectileCounts <= 1f)
                    projectileCounts = 1f;
                return MathF.Tau * SkullIndex / projectileCounts + (Timer / 120f);
            }
        }

        public new string LocalizationCategory => "Projectiles.Misc";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Type] = 2;
            ProjectileID.Sets.TrailCacheLength[Type] = 45;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.netImportant = true;
            Projectile.penetrate = -1;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 9000;
            Projectile.scale = 0f;
        }

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(SkullIndex);

        public override void ReceiveExtraAI(BinaryReader reader) => SkullIndex = reader.ReadInt32();

        public override bool? CanDamage() => false;

        public override void AI()
        {
            if (Owner is null || Owner.dead || !Owner.active || !Owner.HasBuff(ModContent.BuffType<CurseOfNecromancy>()))
            {
                Projectile.Kill();
                return;
            }

            // Hover around the player.
            Vector2 hoverPosition = Owner.MountedCenter + SkullHoverOffsetAngle.ToRotationVector2() * 45f;
            Projectile.Center = Vector2.Lerp(Projectile.Center, hoverPosition, 0.07f);
            Projectile.velocity *= 0.9f;

            // Teleport if the player is too far away.
            if (!Projectile.WithinRange(Owner.Center, 2500f))
            {
                Projectile.Center = hoverPosition;
                Projectile.netUpdate = true;
            }

            Projectile.scale = MathHelper.Clamp(Projectile.scale + 0.03f, 0f, 1f);
            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 velocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(2f, 8f);
                Color color = Color.Lerp(Color.MediumPurple, Color.Magenta, 0.4f);
                float scale = Main.rand.NextFloat(0.25f, 1.25f);
                HeavySmokeParticle heavySmoke = new(Projectile.Center, velocity, color, Main.rand.Next(75, 140), scale, Main.rand.NextFloat(0.35f, 1f), 0.06f, true);
                heavySmoke.SpawnCasParticle();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawChain();
            DrawSkull();
            return false;
        }

        public void DrawSkull()
        {
            Texture2D skullTexture = TextureAssets.Projectile[Type].Value;

            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle rec = skullTexture.Frame();
            Vector2 origin = rec.Size() / 2f;

            Color drawColor = Color.Lerp(Color.MediumPurple, Color.Magenta, 0.4f);
            Main.EntitySpriteDraw(skullTexture, drawPosition, null, drawColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0f);
        }

        public void DrawChain()
        {
            Texture2D chainTexture = ModContent.Request<Texture2D>(Texture + "_Chain").Value;

            Vector2 playerCenter = Owner.MountedCenter;
            Vector2 center = Projectile.Center;
            Vector2 directionToPlayer = playerCenter - Projectile.Center;

            float rotationTowardsPlayer = directionToPlayer.ToRotation() - MathHelper.PiOver2;
            float distanceFromPlayer = directionToPlayer.Length();

            while (distanceFromPlayer > 16f && !float.IsNaN(distanceFromPlayer))
            {
                directionToPlayer /= distanceFromPlayer;
                directionToPlayer *= chainTexture.Height;

                center += directionToPlayer;
                directionToPlayer = playerCenter - center;
                distanceFromPlayer = directionToPlayer.Length();

                Color drawColor = Color.Lerp(Color.MediumPurple, Color.Magenta, 0.4f);
                Main.EntitySpriteDraw(chainTexture, center - Main.screenPosition, chainTexture.Bounds, drawColor, rotationTowardsPlayer, chainTexture.Size() / 2f, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
