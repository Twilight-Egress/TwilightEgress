using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;
using TwilightEgress.Core;
using TwilightEgress.Core.Globals.GlobalNPCs;
using TwilightEgress.Core.Globals.GlobalProjectiles;

namespace TwilightEgress.Content.Items.Dedicated.MPG
{
    public class MoonSpiritKhakkharaHoldout : ModProjectile, ILocalizedModType
    {
        private enum AttackState
        {
            SummoningLanterns,
            RequiemBouquet
        }

        private Player Owner => Main.player[Projectile.owner];

        private ref float Timer => ref Projectile.ai[0];

        private const int MaxChargeTime = 60;

        private const int MaxRequiemBouquetTime = 90;

        private const int RitualCircleOpacityIndex = 0;

        private const int RitualCircleRotationIndex = 1;

        private const int RitualCircleScaleIndex = 2;

        private bool ShouldDespawn => Owner.dead || Owner.CCed || !Owner.active || Owner.HeldItem.type != ModContent.ItemType<MoonSpiritKhakkhara>();

        private AttackState State
        {
            get
            {
                if (Projectile.ai[1] == 1f)
                    return AttackState.RequiemBouquet;
                return AttackState.SummoningLanterns;
            }

            set
            {
                Projectile.ai[1] = (float)value;
            }
        }

        private string[] ViableEasterEggNames = new string[]
        {
            "MPG",
            "TestPG"
        };

        public new string LocalizationCategory => "Projectiles.Summon";

        public override string Texture => "TwilightEgress/Assets/Textures/Items/Dedicated/MPG/MoonSpiritKhakkhara";

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.extraUpdates = 1;
            Projectile.netImportant = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 2700;
        }

        public override void AI()
        {
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];
            ref float ritualCircleRotation = ref Projectile.TwilightEgress().ExtraAI[RitualCircleRotationIndex];
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];

            if (ShouldDespawn)
            {
                Projectile.Kill();
                return;
            }

            switch (State)
            {
                case AttackState.SummoningLanterns:
                    DoBehavior_SummoningLanterns();
                    break;

                case AttackState.RequiemBouquet:
                    DoBehavior_RequiemBouquet();
                    break;
            }

            ritualCircleRotation += 0.03f;

            Timer++;
            Projectile.Center = Owner.Center;
            Projectile.rotation = Projectile.AngleTo(Main.MouseWorld);
            UpdatePlayerVariables(Owner);
        }

        public void DoBehavior_SummoningLanterns()
        {
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];

            bool isChanneling = Owner.channel && Owner.active && Owner.HeldItem.type == ModContent.ItemType<MoonSpiritKhakkhara>();
            if (!isChanneling)
            {
                Projectile.Kill();
                return;
            }

            ritualCircleOpacity = MathHelper.Clamp(ritualCircleOpacity + 0.01f, 0f, 1f);
            ritualCircleScale = MathHelper.Clamp(ritualCircleScale + 0.02f, 0f, 1f);

            // Spawn in the lanterns.
            if (Timer >= MaxChargeTime && Timer % 30 == 0)
            {
                Owner.AddBuff(ModContent.BuffType<UnderworldLanternBuff>(), 180000);
                Vector2 spawnPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f;
                Projectile.BetterNewProjectile(spawnPosition, Vector2.Zero, ModContent.ProjectileType<UnderworldLantern>(), Projectile.damage, 0f, SoundID.DD2_BetsyFireballShot, null, Projectile.owner);

                // Some light dust visuals.
                for (int i = 0; i < 15; i++)
                {
                    Vector2 dustVelocity = Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(spawnPosition, 267, dustVelocity * 5f, newColor: Color.LightSkyBlue, Scale: 2f);
                    dust.noGravity = true;
                }
            }
        }

        public void DoBehavior_RequiemBouquet()
        {
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];

            // Ritual circle visuals.
            if (Timer <= 60f)
                ritualCircleOpacity = MathHelper.Lerp(1f, 0f, EasingFunctions.SineEaseOut(Timer / 60f));
            if (Timer <= 75f)
                ritualCircleScale = MathHelper.Lerp(0f, 3f, EasingFunctions.SineEaseOut(Timer / 75f));

            if (Timer == 1)
            {
                // Spawn the giant lanterns.
                for (int i = 0; i < 2; i++)
                {
                    Vector2 spawnPosition = Owner.Center + Main.rand.NextVector2CircularEdge(120f, 120f);
                    Vector2 initialVelocity = Projectile.SafeDirectionTo(Main.MouseWorld, Vector2.UnitY) * 16f;
                    Projectile.BetterNewProjectile(spawnPosition, initialVelocity, ModContent.ProjectileType<MassiveUnderworldLantern>(), (int)(Projectile.damage * 15f), Projectile.knockBack * 3f, SoundID.Item107, null, Projectile.owner);
                }

                // If the player matches the Easter Egg Name Criteria,
                // play a special sound and shoot up some text.
                if (ViableEasterEggNames.Contains(Owner.name))
                {
                    CombatText.NewText(Owner.Hitbox, Color.SkyBlue, "Requiem Bouquet", true);
                    SoundEngine.PlaySound(AssetRegistry.Sounds.RequiemBouquetPerish, Owner.Center);
                }
            }

            if (Timer >= MaxRequiemBouquetTime + 5)
                Projectile.Kill();
        }

        public void UpdatePlayerVariables(Player owner)
        {
            owner.heldProj = Projectile.whoAmI;
            owner.itemTime = 2;
            owner.itemAnimation = 2;
            owner.ChangeDir(MathF.Sign(Projectile.rotation.ToRotationVector2().X));
            owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.PiOver2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];

            DrawStaff();
            DrawRitualCircle();
            // I'm not sure why this needs to be here personally but
            // not calling this causes the player's arm to also have 
            // the shader applied to them so... lmao?
            Main.spriteBatch.ResetToDefault();

            return false;
        }

        public void DrawStaff()
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            // Essentially, if we face left, flip the sprite.
            SpriteEffects effects = Owner.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // If we're facing the left, we set the extra angle to Pi/2 (90 degrees) so that the rotation flips.
            float extraAngle = Owner.direction < 0 ? MathHelper.PiOver2 : 0f;
            // Set the base draw angle to the projectile's rotation. Projectile.rotation is what
            // you'll be adjusting to change rotations.
            float baseDrawAngle = Projectile.rotation;
            // We set the final rotation by adding the extra angle and Pi/4 (45 degrees) to the base draw angle.
            float drawRotation = baseDrawAngle + MathHelper.PiOver4 + extraAngle;

            // Set the origin that we'll draw from. 
            // This code here specifically ensures that the sprite itself flips depending on what direction
            // we are facing.
            Vector2 origin = new Vector2(Owner.direction < 0 ? texture.Width : 0f, texture.Height);
            // Get the position we will draw on.
            // We use the projectile's center and add the base draw angle, converted to a Vector2, to it and then
            // subtract the screen position.
            Vector2 drawPosition = Projectile.Center + baseDrawAngle.ToRotationVector2() - Main.screenPosition;

            // Draw pulsing backglow effects.
            Main.spriteBatch.UseBlendState(BlendState.Additive);
            for (int i = 0; i < 4; i++)
            {
                Vector2 backglowDrawPositon = drawPosition + Vector2.UnitY.RotatedBy(i * MathHelper.TwoPi / 4) * 3f;
                Color backglowColor = Utilities.ColorSwap(Color.Cyan, Color.LightSkyBlue, 5f);
                Main.EntitySpriteDraw(texture, backglowDrawPositon, null, Projectile.GetAlpha(backglowColor), drawRotation, origin, Projectile.scale, effects, 0);
            }
            Main.spriteBatch.ResetToDefault();

            // Draw the main sprite.
            Main.EntitySpriteDraw(texture, drawPosition, null, Projectile.GetAlpha(Color.White), drawRotation, origin, Projectile.scale, effects, 0);
        }

        public void DrawRitualCircle()
        {
            ref float ritualCircleOpacity = ref Projectile.TwilightEgress().ExtraAI[RitualCircleOpacityIndex];
            ref float ritualCircleRotation = ref Projectile.TwilightEgress().ExtraAI[RitualCircleRotationIndex];
            ref float ritualCircleScale = ref Projectile.TwilightEgress().ExtraAI[RitualCircleScaleIndex];

            Texture2D ritualCircle = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/RancorMagicCircle").Value;
            Texture2D blurredRitualCircle = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Magic/RancorMagicCircleGlowmask").Value;

            // Summoning Circle.
            Vector2 ritualCircleDrawPosition = Projectile.Center + Projectile.rotation.ToRotationVector2() * 85f - Main.screenPosition;

            ApplyShader(blurredRitualCircle, ritualCircleOpacity, -ritualCircleRotation, Projectile.AngleTo(Main.MouseWorld), -1, Color.SkyBlue, Color.DarkCyan, BlendState.Additive);
            Main.EntitySpriteDraw(blurredRitualCircle, ritualCircleDrawPosition, null, Color.White, 0f, blurredRitualCircle.Size() / 2f, ritualCircleScale * 1.275f, SpriteEffects.None, 0);
            ApplyShader(ritualCircle, ritualCircleOpacity, ritualCircleRotation, Projectile.AngleTo(Main.MouseWorld), -1, Color.CornflowerBlue, Color.LightSkyBlue, BlendState.AlphaBlend);
            Main.EntitySpriteDraw(ritualCircle, ritualCircleDrawPosition, null, Color.White, 0f, ritualCircle.Size() / 2f, ritualCircleScale, SpriteEffects.None, 0);
            Main.spriteBatch.ResetToDefault();
        }

        public static void ApplyShader(Texture2D texture, float opacity, float circularRotation, float directionRotation, int direction, Color startingColor, Color endingColor, BlendState blendMode)
        {
            Main.spriteBatch.PrepareForShaders(blendMode);

            CalamityUtils.CalculatePerspectiveMatricies(out var viewMatrix, out var projectionMatrix);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseColor(startingColor);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseSecondaryColor(endingColor);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseSaturation(directionRotation);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uDirection"].SetValue(direction);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uCircularRotation"].SetValue(circularRotation);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uImageSize0"].SetValue(texture.Size());
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["overallImageSize"].SetValue(texture.Size());
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].Shader.Parameters["uWorldViewProjection"].SetValue(viewMatrix * projectionMatrix);
            GameShaders.Misc["CalamityMod:RancorMagicCircle"].Apply();
        }
    }
}
