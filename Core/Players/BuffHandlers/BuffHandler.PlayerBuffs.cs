﻿using CalamityMod;
using Luminance.Common.Utilities;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using TwilightEgress.Content.Items.Dedicated.Enchilada;

namespace TwilightEgress.Core.Players.BuffHandlers
{
    public partial class BuffHandler
    {
        #region Minion Buffs
        public bool MoonSpiritLantern { get; set; }

        public bool GeminiGenies { get; set; }
        public bool GeminiGeniesVanity { get; set; }

        public bool OctoKibby { get; set; }
        public bool OctoKibbyVanity { get; set; }
        #endregion

        #region Misc. Buffs
        public static List<bool> MechonSlayerBuffs { get; set; }
        #endregion

        #region Other Fields and Properties
        private int MechonSlayerResetTime;

        private readonly int MechonSlayerMaxResetTime = Utilities.SecondsToFrames(5);
        #endregion

        #region Methods
        private void HandlePlayerBuffEffects()
        {
            // Mechon Slayer's various buff effects.
            MechonSlayerBuffEffects();
        }

        private void MechonSlayerBuffEffects()
        {
            if (MechonSlayerBuffs[0])
            {
                Player.statDefense += 15;
                Player.Calamity().contactDamageReduction += 0.5D;
                Player.Calamity().projectileDamageReduction += 0.5D;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                    Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);

                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.OrangeTorch, dustVelocity.SafeNormalize(Vector2.Zero));
                    dust.noGravity = true;
                }
            }

            if (MechonSlayerBuffs[1] && !Player.HasCooldown(MechonSlayerEater.ID))
            {
                // Remove all debuffs from the player.
                for (int i = 0; i < Player.MaxBuffs; i++)
                {
                    if (Player.buffType[i] <= 0 || !Main.debuff[Player.buffType[i]] || Player.buffType[i] == BuffID.PotionSickness)
                        continue;

                    Player.buffTime[i] = 0;
                    Player.buffType[i] = 0;
                }

                // Apply a separate cooldown specifically for this effect.
                Player.AddCooldown(MechonSlayerEater.ID, Utilities.SecondsToFrames(120));
            }

            if (MechonSlayerBuffs[2])
            {
                Player.GetDamage(Player.HeldItem.DamageType) += 0.10f;
                Player.GetArmorPenetration(Player.HeldItem.DamageType) += 10f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                    Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);

                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.PurpleTorch, dustVelocity.SafeNormalize(Vector2.Zero));
                    dust.noGravity = true;
                }
            }

            if (MechonSlayerBuffs[3])
            {
                Player.Calamity().contactDamageReduction += 0.10D;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                    Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);

                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.GreenTorch, dustVelocity.SafeNormalize(Vector2.Zero));
                    dust.noGravity = true;
                }
            }

            if (MechonSlayerBuffs[4])
            {
                Player.maxRunSpeed *= 1.15f;
                Player.runAcceleration *= 1.05f;

                for (int i = 0; i < 2; i++)
                {
                    Vector2 dustPosition = Player.Center + Main.rand.NextVector2Circular(Player.width, Player.height);
                    Vector2 dustVelocity = Vector2.UnitY * Main.rand.NextFloat(-8f, -2f);

                    Dust dust = Dust.NewDustPerfect(dustPosition, DustID.IceTorch, dustVelocity.SafeNormalize(Vector2.Zero));
                    dust.noGravity = true;
                }
            }

            if (!Player.HasCooldown(MechonSlayerArtSelection.ID))
                MechonSlayerResetTime++;
        }
        #endregion
    }
}
