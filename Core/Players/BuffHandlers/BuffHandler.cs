using CalamityMod;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Content.Items.Dedicated.Enchilada;

namespace TwilightEgress.Core.Players.BuffHandlers
{
    public partial class BuffHandler : ModPlayer
    {
        public override void Initialize()
        {
            MechonSlayerBuffs = new List<bool>()
            {
                false,      // 0th index - Armor.
                false,      // 1st index - Eater.
                false,      // 2nd index - Enchant.
                false,      // 3rd index - Purge
                false       // 4th index - Speed.
            };
        }

        public static void StuffToUnload()
        {
            MechonSlayerBuffs = null;
        }

        public override void UpdateDead()
        {
            ResetNeccessaryVariables();
            VariablesToBeResetOnDeath();
        }

        public override void ResetEffects() => ResetNeccessaryVariables();

        public override void PostUpdateMiscEffects()
        {
            // Handles all non-regen affecting buff effects.
            HandlePlayerBuffEffects();

            // Handles all non-regen affecting debuff effects.
            HandlePlayerDebuffs();
        }

        public void ApplyMechonSlayerArt(int artID)
        {
            if (Player.HasCooldown(MechonSlayerArtSelection.ID) || artID <= -1)
                return;

            // Disable all arts before renabling the art at the specified index.
            for (int i = 0; i < MechonSlayerBuffs.Count; i++)
                MechonSlayerBuffs[i] = false;
            MechonSlayerBuffs[artID] = true;

            if (artID != 1)
                Player.AddCooldown(MechonSlayerArtSelection.ID, Luminance.Common.Utilities.Utilities.SecondsToFrames(30));
            MechonSlayerResetTime = 0;
        }

        private void ResetNeccessaryVariables()
        {
            // Minion buffs.
            MoonSpiritLantern = false;
            GeminiGenies = false;
            GeminiGeniesVanity = false;
            OctoKibby = false;
            OctoKibbyVanity = false;

            // Misc buffs.
            if (MechonSlayerResetTime >= MechonSlayerMaxResetTime)
            {
                for (int i = 0; i < MechonSlayerBuffs.Count; i++)
                    MechonSlayerBuffs[i] = false;
            }

            // Misc debuffs.
            CerebralMindtrick = false;
            CurseOfNecromancy = false;
            BellbirdStun = false;
        }

        private void VariablesToBeResetOnDeath()
        {
            CurseOfNecromancyMinionSlotStack = 0;
            BellbirdStunTime = 0;
        }
    }
}
