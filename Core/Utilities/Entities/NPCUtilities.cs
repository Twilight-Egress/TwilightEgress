using System.Linq;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Utilities;
using static Terraria.Utilities.NPCUtils;

namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// A method used for spawning projectiles that also caters for updating projectiles on the network and playing sounds.
        /// </summary>
        /// <param name="spawnX">The x spawn position of the projectile.</param>
        /// <param name="spawnY">The y spawn position of the projectile.</param>
        /// <param name="velocityX">The x velocity of the projectile.</param>
        /// <param name="velocityY">The y velocity of the projectile.</param>
        /// <param name="type">The id of the projectile type that is being summoned.</param>
        /// <param name="damage">The damage of the projectile that is being summoned.</param>
        /// <param name="knockback">The knockback of the projectile that is being summoned.</param>
        /// <param name="sound">The SoundStyle ID of the sound that should be played. Defaults to null.</param>
        /// <param name="source">The <see cref="IEntitySource"/> of the projectile being spawned. This parameter is optional and will default
        /// to <see cref="Entity.GetSource_FromAI(string?)"/> if left null.</param>
        /// <param name="owner">The owner of the projectile that is being summond. Defaults to Main.myPlayer.</param>
        /// <param name="ai0">An optional <see cref="Projectile.ai"/>[0] fill value. Defaults to 0.</param>
        /// <param name="ai1">An optional <see cref="Projectile.ai"/>[1] fill value. Defaults to 0.</param>
        /// <param name="ai2">An optional <see cref="Projectile.ai"/>[2] fill value. Defaults to 0.</param>
        /// <returns>The index of the projectile being spawned.</returns>
        public static int BetterNewProjectile(this NPC npc, float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, SoundStyle? sound = null, IEntitySource source = null, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            if (owner == -1)
                owner = Main.myPlayer;

            if (sound.HasValue)
                SoundEngine.PlaySound(sound, npc.Center);

            float damageCorrection = 0.5f;
            if (Main.expertMode)
                damageCorrection = 0.25f;
            if (Main.masterMode)
                damageCorrection = 0.1667f;
            damage = (int)(damage * damageCorrection);

            int index = Projectile.NewProjectile(source ?? npc.GetSource_FromAI(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1, ai2);
            if (Main.projectile.IndexInRange(index))
                Main.projectile[index].netUpdate = true;

            return index;
        }

        /// <summary>
        /// A method used for spawning projectiles that also caters for updating projectiles on the network and playing sounds.
        /// This iteration in particular accepts a Vector2 for both the spawn position and velocity paramters.
        /// </summary>
        /// <param name="center">The spawn positon of the projectile.</param>
        /// <param name="velocity">The velocity of the projectile.</param>
        /// <param name="type">The id of the projectile type that is being summoned.</param>
        /// <param name="damage">The damage of the projectile that is being summoned.</param>
        /// <param name="knockback">The knockback of the projectile that is being summoned.</param>
        /// <param name="sound">The SoundStyle ID of the sound that should be played. Defaults to null.</param>
        /// <param name="source">The <see cref="IEntitySource"/> of the projectile being spawned. This parameter is optional and will default
        /// to <see cref="Entity.GetSource_FromAI(string?)"/> if left null.</param>
        /// <param name="owner">The owner of the projectile that is being summond. Defaults to Main.myPlayer.</param>
		/// <param name="ai0">An optional <see cref="Projectile.ai"/>[0] fill value. Defaults to 0.</param>
		/// <param name="ai1">An optional <see cref="Projectile.ai"/>[1] fill value. Defaults to 0.</param>
        /// <param name="ai2">An optional <see cref="Projectile.ai"/>[2] fill value. Defaults to 0.</param>
        /// <returns>The index of the projectile being spawned.</returns>
        public static int BetterNewProjectile(this NPC npc, Vector2 center, Vector2 velocity, int type, int damage, float knockback, SoundStyle? sound = null, IEntitySource source = null, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return npc.BetterNewProjectile(center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, sound, source, owner, ai0, ai1, ai2);
        }

        /// <summary>
		/// A custom version of <see cref="NPC.NewNPC"/> that handles updating projectiles on the network and changes the 
        /// spawn variables to be consistent with <see cref="Projectile.NewProjectile"/>.
		/// </summary>
		/// <param name="spawnX">The X spawn position of the NPC being spawned.</param>
		/// <param name="spawnY">The Y spawn position of the NPC being spawned.</param>
		/// <param name="type">The ID of the NPC that is being spawned.</param>
		/// <param name="initialSpawnSlot">Can be used to ensure that the NPC you are spawning is spawned in a slot after an existing NPC. E.g. Ensuring that Boss Minions draw behind the main Boss. Defaults to 0.</param>
		/// <param name="ai0">An optional <see cref="NPC.ai"/>[0] fill value. Defaults to 0.</param>
		/// <param name="ai1">An optional <see cref="NPC.ai"/>[1] fill value. Defaults to 0.</param>
		/// <param name="ai2">An optional <see cref="NPC.ai"/>[2] fill value. Defaults to 0.</param>
		/// <param name="ai3">An optional <see cref="NPC.ai"/>[3] fill value. Defaults to 0.</param>
		/// <param name="target">Can be set to a <see cref="Player.whoAmI"/> to have the NPC being spawned to target a specific player immediately. Defaults to 255.</param>
		/// <param name="velocity">Can be used to give the NPC a new velocity value immediatley upon spawning.</param>
		public static int BetterNewNPC(this NPC npc, float spawnX, float spawnY, int type, IEntitySource source = null, int initialSpawnSlot = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            int index = NPC.NewNPC(source ?? npc.GetSource_FromAI(), (int)spawnX, (int)spawnY, type, initialSpawnSlot, ai0, ai1, ai2, ai3, target);
            if (Main.npc.IndexInRange(index))
            {
                Main.npc[index].velocity = velocity;
                Main.npc[index].netUpdate = true;
            }
            return index;
        }

        /// <summary>
        /// A custom version of <see cref="NPC.NewNPC"/> that changes the spawn variables to be consistent with <see cref="Projectile.NewProjectile"/>.
        /// Also contains a new velocity paramter that allows you to set NPC velocity right as it spawns.
        /// This particular iteration accepts a Vector2 for the spawn position of the NPC.
        /// </summary>
        /// <param name="spawn">The Vector2 position of the NPC being spawned.</param>
        /// <param name="type">The ID of the NPC that is being spawned.</param>
        /// <param name="initialSpawnSlot">Can be used to ensure that the NPC you are spawning is spawned in a slot after an existing NPC. E.g. Ensuring that Boss Minions draw behind the main Boss. Defaults to 0.</param>
        /// <param name="ai0">An optional <see cref="NPC.ai"/>[0] fill value. Defaults to 0.</param>
        /// <param name="ai1">An optional <see cref="NPC.ai"/>[1] fill value. Defaults to 0.</param>
        /// <param name="ai2">An optional <see cref="NPC.ai"/>[2] fill value. Defaults to 0.</param>
        /// <param name="ai3">An optional <see cref="NPC.ai"/>[3] fill value. Defaults to 0.</param>
        /// <param name="target">Can be set to a <see cref="Player.whoAmI"/> to have the NPC being spawned to target a specific player immediately. Defaults to 255.</param>
        /// <param name="velocity">Can be used to give the NPC a new velocity value immediatley upon spawning.</param>
        public static int BetterNewNPC(this NPC npc, Vector2 spawn, int type, IEntitySource source = null, int initialSpawnSlot = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            return npc.BetterNewNPC(spawn.X, spawn.Y, type, source, initialSpawnSlot, ai0, ai1, ai2, ai3, target, velocity);
        }

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
        public static void AdvancedNPCTargeting(this NPC searcher, bool targetPlayers, float maxPlayerSearchDistance, bool targetNPCs,  float maxNPCSearchDistance, params int[] specificNPCsToTarget)
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
