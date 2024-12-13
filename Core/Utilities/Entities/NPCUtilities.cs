using Microsoft.Xna.Framework;
using System;
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

        /// <summary>
        /// Creates an arc around an entity from its center which checks if they should turn around due to either approaching tiles or if they are
        /// about to leave the bounds of space.
        /// </summary>
        /// <param name="minRadians">The minimum angle of the arc in radians.</param>
        /// <param name="maxRadians">The maximum angle of the arc in radians</param>
        /// <param name="radiansIncrement">By how many radians should the loop increment by when looping between the two angles.</param>
        /// <param name="shouldTurnAround">Whether or not the entity should turn around or not.</param>
        /// <param name="checkDirection">The direction of the arc in the form of a Vector2. This will be automatically safe normalized and rotated 
        /// so there is no need for you to do so outside of this function. Defaults to using the entity's velocity if no value is input.</param>
        /// <param name="checkRadius">The radius of the arc from the center of the entity</param>
        public static void CheckForTurnAround(this Entity entity, float minRadians, float maxRadians, float radiansIncrement, out bool shouldTurnAround, Vector2? checkDirection = null, float checkRadius = 60f)
        {
            shouldTurnAround = false;
            for (float i = minRadians; i < maxRadians; i += radiansIncrement)
            {
                Vector2 arcVector = checkDirection.HasValue ? checkDirection.Value.SafeNormalize(Vector2.Zero).RotatedBy(i) :
                    entity.velocity.SafeNormalize(Vector2.Zero).RotatedBy(i);

                if (!Collision.CanHit(entity.Center, 1, 1, entity.Center + arcVector * 60f, 1, 1))
                {
                    shouldTurnAround = true;
                    break;
                }
            }
        }

        public static NPC FindClosestNPC(this NPC npc, out float distanceToNPC, params int[] typesToSearchFor)
        {
            NPC closestMatch = null;
            distanceToNPC = 9999999f;

            if (npc is null || !npc.active)
                return null;

            foreach (NPC activeNPC in Main.ActiveNPCs)
            {
                if (!typesToSearchFor.Contains(activeNPC.type))
                    continue;

                bool canHit = Collision.CanHit(npc.Center, 1, 1, activeNPC.Center, 1, 1);
                if (Vector2.DistanceSquared(npc.Center, activeNPC.Center) < distanceToNPC && canHit && activeNPC.whoAmI != npc.whoAmI)
                {
                    distanceToNPC = Vector2.DistanceSquared(npc.Center, activeNPC.Center);
                    closestMatch = activeNPC;
                }
            }

            // Square root the distance to the closest NPC.
            distanceToNPC = MathF.Sqrt(distanceToNPC);
            return closestMatch;
        }

        public static void AdjustNPCHitboxToScale(this NPC npc, float originalWidth, float originalHeight)
        {
            int oldWidth = npc.width;
            int idealWidth = (int)(npc.scale * originalWidth);
            int idealHeight = (int)(npc.scale * originalHeight);
            if (idealWidth != oldWidth)
            {
                npc.position.X += npc.width / 2;
                npc.position.Y += npc.height / 2;
                npc.width = idealWidth;
                npc.height = idealHeight;
                npc.position.X -= npc.width / 2;
                npc.position.Y -= npc.height / 2;
            }
        }
    }
}
