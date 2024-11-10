using TwilightEgress.Content.Events;
using TwilightEgress.Content.Events.CosmostoneShowers;

namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        public static bool ZoneCosmostoneShowers(this Player player) => EventHandlerManager.SpecificEventIsActive<CosmostoneShowerEvent>() && (player.ZoneOverworldHeight || player.ZoneSkyHeight);

        /// <summary>
        /// Checks if a player is alive and is channeling while holding a specific item.
        /// </summary>
        /// <returns>Whether or not the player's alive, properly channeling and holding the correct item.</returns>
        public static bool PlayerIsChannelingWithItem(this Player player, int specificItemType, bool rightClick = false) 
            => player.active && (rightClick ? player.Calamity().mouseRight : player.channel) && player.HeldItem.type == specificItemType;

        /// <summary>
        /// Compiles a few commonly used checks to determine whether a player's held projectile should be despawned or not.
        /// </summary>
        public static bool ShouldDespawnHeldProj(this Player player, int heldItemType) => player.dead || player.CCed || player.noItems || !player.active || player.HeldItem.type != heldItemType;

        /// <summary>
        /// A copy of Calamity's ConsumeRogueStealth method. This is only to be used if your Rogue Weapon functions under a held projectile or some other mean
        /// that isn't taken into consideration during manual stealth updating. This was made because the original method is internal.
        /// </summary>
        public static void ConsumeStealthManually(this Player owner)
        {
            owner.Calamity().stealthStrikeThisFrame = true;
            owner.Calamity().stealthAcceleration = 1f;
            float lossReductionRatio = (float)owner.Calamity().flatStealthLossReduction / (owner.Calamity().rogueStealthMax * 100f);
            float remainingStealth = owner.Calamity().rogueStealthMax * lossReductionRatio;
            float stealthToLose = owner.Calamity().rogueStealthMax - remainingStealth;
            if (stealthToLose < 0.01f)
            {
                stealthToLose = 0.01f;
            }
            if (owner.Calamity().stealthStrikeHalfCost)
            {
                owner.Calamity().rogueStealth -= 0.5f * stealthToLose;
                if (owner.Calamity().rogueStealth <= 0f)
                {
                    owner.Calamity().rogueStealth = 0f;
                }
            }
            else if (owner.Calamity().stealthStrike75Cost)
            {
                owner.Calamity().rogueStealth -= 0.75f * stealthToLose;
                if (owner.Calamity().rogueStealth <= 0f)
                {
                    owner.Calamity().rogueStealth = 0f;
                }
            }
            else if (owner.Calamity().stealthStrike85Cost)
            {
                owner.Calamity().rogueStealth -= 0.9f * stealthToLose;
                if (owner.Calamity().rogueStealth <= 0f)
                {
                    owner.Calamity().rogueStealth = 0f;
                }
            }
            else
            {
                owner.Calamity().rogueStealth = remainingStealth;
            }
        }

        /// <summary>
        /// Manually consumes the player's mana and sets a mana regeneration delay.
        /// </summary>
        /// <param name="manaAmount">The amount of mana that should be consumed.</param>
        /// <param name="maxDelay">The maximum amount of downtime before mana begins to regenerate. Defaults to <see cref="Player.maxRegenDelay"/> if no value is input.</param>
        public static void ConsumeManaManually(this Player player, int manaAmount, float? maxDelay = null)
        {
            if (player.CheckMana(manaAmount, true, false))
            {
                maxDelay ??= player.maxRegenDelay;
                player.manaRegenDelay = maxDelay.Value;
            }
        }   
    }
}
