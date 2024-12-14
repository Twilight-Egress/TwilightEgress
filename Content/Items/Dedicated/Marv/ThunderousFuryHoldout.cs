using CalamityMod;
using CalamityMod.Sounds;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Content.Particles;
using TwilightEgress.Core;

namespace TwilightEgress.Content.Items.Dedicated.Marv
{
    public class ThunderousFuryHoldout : ModProjectile, ILocalizedModType
    {
        private Player Owner => Main.player[Projectile.owner];

        private ref float ChargeTimer => ref Projectile.ai[0];

        private ref float DelayTimer => ref Projectile.ai[1];

        private ref float AttackType => ref Projectile.ai[2];

        private const int DelayBeforeFiring = 75;

        private const int FireRate = 60;

        private const int MaxManaForLeftClick = 15;

        private const int MaxManaForRightClick = 100;

        private bool IsManaThresholdMet
        {
            get
            {
                if (Owner.Calamity().mouseRight)
                    return Owner.statMana > MaxManaForRightClick;
                return Owner.statMana > MaxManaForLeftClick;
            }
        }

        private string[] ViableEasterEggNames = new string[]
        {
            "Marv",
            "ThatOneEmolgaLiker"
        };

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "TwilightEgress/Assets/Textures/Items/Dedicated/Marv/ThunderousFury";

        public override void SetDefaults()
        {
            Projectile.width = 124;
            Projectile.height = 52;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }

        public override void AI()
        {
            bool isChanneling = (Owner.channel || Owner.Calamity().mouseRight) && Owner.active && Owner.HeldItem.type == ModContent.ItemType<ThunderousFury>();
            bool shouldDespawn = !isChanneling || !IsManaThresholdMet || Owner.CCed || Owner.dead;
            if (shouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            switch (AttackType)
            {
                case 0:
                    DoBehavior_Thunderbolt();
                    break;

                case 1:
                    DoBehavior_BoltStrike();
                    break;
            }

            DelayTimer++;
            UpdateProjectileSpecificVariables(Owner);
            UpdatePlayerSpecificVariables(Owner);
        }

        public void UpdateProjectileSpecificVariables(Player owner)
        {
            Projectile.Center = owner.RotatedRelativePoint(owner.MountedCenter, true);
            Projectile.rotation = owner.MountedCenter.AngleTo(owner.Calamity().mouseWorld);
            if (Projectile.spriteDirection == -1)
                Projectile.rotation += MathHelper.Pi;
        }

        public void DoBehavior_Thunderbolt()
        {
            if (DelayTimer == 1)
            {
                SoundStyle thunderboltStartSound = ViableEasterEggNames.Contains(Owner.name) ? AssetRegistry.Sounds.PikachuCry : CommonCalamitySounds.LightningSound;
                SoundEngine.PlaySound(thunderboltStartSound, Projectile.Center);

                Color particleColor = Color.Lerp(Color.Yellow, Color.Goldenrod, Main.rand.NextFloat());
                int sparkLifespan = Main.rand.Next(20, 36);
                float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                for (int i = 0; i < 50; i++)
                {
                    Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 16f);
                    SparkParticle electricSpark = new(Projectile.Center, sparkVelocity, particleColor, sparkScale, sparkLifespan);
                    electricSpark.SpawnCasParticle();
                }
            }

            if (DelayTimer >= DelayBeforeFiring)
            {
                if (ChargeTimer % FireRate == 0)
                {
                    if (Owner.CheckMana(15, true, false))
                        Owner.manaRegenDelay = 75;

                    Vector2 spawnPosition = Main.MouseWorld + new Vector2(Main.rand.NextFloat(-300f, 300f), -900f);
                    Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<ElectricSkyBolt>(), Projectile.damage, Projectile.knockBack, owner: Projectile.owner);
                }
                ChargeTimer++;
            }
        }

        public void DoBehavior_BoltStrike()
        {
            if (DelayTimer == 1)
            {
                SoundStyle boltStrikeStartSound = ViableEasterEggNames.Contains(Owner.name) ? AssetRegistry.Sounds.ZekromCry : CommonCalamitySounds.ExoPlasmaShootSound;
                SoundEngine.PlaySound(boltStrikeStartSound, Projectile.Center);

                Color particleColor = Color.Lerp(Color.Cyan, Color.SkyBlue, Main.rand.NextFloat());
                int sparkLifespan = Main.rand.Next(20, 36);
                float sparkScale = Main.rand.NextFloat(0.75f, 1.25f);
                for (int i = 0; i < 50; i++)
                {
                    Vector2 sparkVelocity = Vector2.UnitX.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(9f, 16f);
                    SparkParticle electricSpark = new(Projectile.Center, sparkVelocity, particleColor, sparkScale, sparkLifespan);
                    electricSpark.SpawnCasParticle();
                }
            }

            if (DelayTimer == DelayBeforeFiring)
            {
                if (Owner.CheckMana(100, true, false))
                    Owner.manaRegenDelay = 75;

                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 120f;
                Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<BoltStrike>(), Projectile.damage, Projectile.knockBack, owner: Projectile.owner);
            }
        }

        public void UpdatePlayerSpecificVariables(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.ChangeDir(Math.Sign(Projectile.rotation.ToRotationVector2().X));
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            float rotation = Projectile.rotation + (Owner.direction < 0 ? MathHelper.Pi : 0f);
            Vector2 drawPosition = Owner.MountedCenter + new Vector2(0f, -2f) + Projectile.rotation.ToRotationVector2() - Main.screenPosition;

            // Draw pulsing backglow effects.
            for (int i = 0; i < 4; i++)
            {
                float backglowRadius = MathHelper.Lerp(2f, 5f, EasingFunctions.SineEaseInOut((float)(Main.timeForVisualEffects / 30f)));
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 4) * backglowRadius;

                Main.spriteBatch.UseBlendState(BlendState.Additive);
                Main.EntitySpriteDraw(texture, backglowDrawPositon, texture.Frame(), Projectile.GetAlpha(Color.LightYellow), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
                Main.spriteBatch.ResetToDefault();
            }

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, texture.Frame(), Projectile.GetAlpha(lightColor), rotation, texture.Size() / 2f, Projectile.scale, effects, 0);
            return false;
        }
    }
}
