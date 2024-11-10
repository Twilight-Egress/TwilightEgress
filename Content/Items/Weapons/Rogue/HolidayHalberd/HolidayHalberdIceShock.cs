using CalamityMod.Buffs.StatDebuffs;

namespace TwilightEgress.Content.Items.Weapons.Rogue.HolidayHalberd
{
    public class HolidayHalberdIceShock : ModProjectile, ILocalizedModType
    {
        private ref float Timer => ref Projectile.ai[0];

        private ref float RandomNewScale => ref Projectile.ai[1];

        private const int MaxLifetime = 75;

        private const int ProjectileTextureOpacityIndex = 0;

        public new string LocalizationCategory => "Projectiles.Rogue";

        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossIceMist;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 92;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.DamageType = ModContent.GetInstance<RogueDamageClass>();
            Projectile.Opacity = 0f;
            Projectile.scale = 0f;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void OnSpawn(IEntitySource source)
        {
            RandomNewScale = Main.rand.NextFloat(1f, 1.75f);
            Projectile.rotation = Main.rand.NextFloat(TwoPi);
            SoundEngine.PlaySound(TwilightEgressSoundRegistry.IceShock, Projectile.Center);
        }

        public override void AI()
        {
            ref float projectileTextureOpacity = ref Projectile.TwilightEgress().ExtraAI[ProjectileTextureOpacityIndex];

            if (Timer >= MaxLifetime)
            {
                Projectile.Kill();
                return;
            }

            // Scale up.
            if (Timer <= 20f)
            {
                Projectile.scale = Lerp(Projectile.scale, RandomNewScale, TwilightEgressUtilities.SineEaseInOut(Timer / 20f));
                Projectile.Opacity = Lerp(Projectile.Opacity, 1f, TwilightEgressUtilities.SineEaseInOut(Timer / 20f));
            }

            if (Timer >= 30f)
                projectileTextureOpacity = Clamp(projectileTextureOpacity + 0.065f, 0f, 1f);

            // Particles.
            if (Main.rand.NextBool(3))
            {
                for (int i = 0; i < 3; i++)
                {
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.width);
                    Color color = Color.Lerp(Color.LightSkyBlue, Color.Cyan, Main.rand.NextFloat());
                    float scale = Main.rand.NextFloat(0.2f, 0.8f) * Projectile.scale;
                    int lifespan = Main.rand.Next(15, 30);
                    SparkleParticle sparkleParticle = new(spawnPosition, Vector2.Zero, color, color * 0.35f, scale, lifespan, 0.25f, 1.25f);
                    sparkleParticle.SpawnCasParticle();
                }
            }

            if (Main.rand.NextBool(5))
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 velocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(3f, 10f);
                    Vector2 spawnPosition = Projectile.Center + Main.rand.NextVector2Circular(Projectile.width, Projectile.width);
                    Color initialColor = Color.Lerp(Color.LightSkyBlue, Color.Cyan, Main.rand.NextFloat()) * Main.rand.NextFloat(0.45f, 0.75f);
                    float scale = Main.rand.NextFloat(0.75f, 2f) * Projectile.scale;
                    float opacity = Main.rand.NextFloat(0.6f, 1f);
                    MediumMistParticle deathSmoke = new(spawnPosition, velocity, initialColor, initialColor * 0.45f, scale, opacity, Main.rand.Next(60, 120), 0.03f);
                    deathSmoke.SpawnCasParticle();
                }
            }

            Timer++;
            Projectile.AdjustProjectileHitboxByScale(92f, 92f);
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(TwilightEgressSoundRegistry.CryogenShieldBreak, Projectile.Center);
            // Spawn a ring of arcing snowflakes, similar to the original Iceshock.
            float snowflakeAngularVelocity = ToRadians(3f);
            for (int i = 0; i < 6; i++)
            {
                Vector2 snowflakeVelocity = Vector2.UnitX.RotatedBy(TwoPi * i / 6) * 16f;
                int damage = Projectile.damage.GetPercentageOfInteger(0.65f);
                Projectile.BetterNewProjectile(Projectile.Center, snowflakeVelocity, ModContent.ProjectileType<HolidayHalberdIceShockSnowflake>(), damage,
                    Projectile.knockBack, owner: Projectile.owner, ai0: snowflakeAngularVelocity);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.Frostburn2, 180);
            if (Main.rand.NextBool(10) && !target.HasBuff(ModContent.BuffType<GlacialState>()))
            {
                target.AddBuff(ModContent.BuffType<GlacialState>(), 180);
                SoundEngine.PlaySound(TwilightEgressSoundRegistry.IceShockPetrify, Projectile.Center);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float projectileTextureOpacity = ref Projectile.TwilightEgress().ExtraAI[ProjectileTextureOpacityIndex];
            Texture2D glowTexture = ModContent.Request<Texture2D>("TwilightEgress/Content/Items/Weapons/Rogue/HolidayHalberd/HolidayHalberdIceShock_Glow").Value;

            // Backglow.
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Projectile.DrawBackglow(Projectile.GetAlpha(Color.LightSkyBlue * 0.45f), 3f);
            Main.spriteBatch.ResetToDefault();

            // White overlay.
            Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, texture: glowTexture);
            // Actual projectile texture.
            Projectile.DrawTextureOnProjectile(Color.White * projectileTextureOpacity, Projectile.rotation, Projectile.scale);

            return false;
        }
    }
}
