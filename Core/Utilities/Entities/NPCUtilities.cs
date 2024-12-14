using System.Linq;
using Terraria;
using static Terraria.Utilities.NPCUtils;

namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Handles boilerplate code which allows NPCs using <see cref="NPCID.Sets.UsesNewTargetting"/> to target both players and NPCs within
        /// a certain distance. 
        /// </summary>
        /// <param name="searcher">The NPC that is searching for targets.</param>
        /// <param name="targetPlayers">Whether or not this NPC should target players.</param>
        /// <param name="maxPlayerSearchDistance">The maximum distance in which this NPC can search for any players.</param>
        /// <param name="targetNPCs">Whether or not this NPC should target NPCs.</param>
        /// <param name="maxNPCSearchDistance">The maximum distance in which this NPC can search for any NPCs.</param>
        /// <param name="specificNPCsToTarget">An optional array to add any specific NPC types that this NPC should target. If this array is used,
        /// the NPC will ONLY target the types specifically found within it. If it is left empty, the NPC will target ANY nearby NPC.</param>
        public static void AdvancedNPCTargeting(this NPC searcher, bool targetPlayers, float maxPlayerSearchDistance, bool targetNPCs, float maxNPCSearchDistance, params int[] specificNPCsToTarget)
        {
            bool playerSearchFilter(Player player)
                => player.WithinRange(searcher.Center, maxPlayerSearchDistance) && targetPlayers;

            bool npcSearchFilter(NPC npc)
            {
                if (specificNPCsToTarget.Length == 0)
                    return npc.WithinRange(npc.Center, maxNPCSearchDistance) && targetNPCs;
                else
                    return specificNPCsToTarget.Contains(npc.type) && npc.WithinRange(npc.Center, maxNPCSearchDistance) && targetNPCs;
            }

            TargetSearchResults results = SearchForTarget(searcher, TargetSearchFlag.All, playerSearchFilter, npcSearchFilter);
            if (results.FoundTarget)
            {
                TargetType targetType = results.NearestTargetType;
                if (results.FoundTank && !results.NearestTankOwner.dead && targetPlayers)
                    targetType = TargetType.Player;

                searcher.target = results.NearestTargetIndex;
                searcher.targetRect = results.NearestTargetHitbox;
            }
        }
    }
}
