using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.DataStructures;
using Terraria;
using Microsoft.Xna.Framework;

namespace TwilightEgress.Core
{
    public static class NPCSpawning
    {
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
		public static int BetterNewNPC(this Entity entity, float spawnX, float spawnY, int type, IEntitySource source = null, int initialSpawnSlot = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            int index = NPC.NewNPC(source ?? entity.GetSource_FromAI(), (int)spawnX, (int)spawnY, type, initialSpawnSlot, ai0, ai1, ai2, ai3, target);
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
        public static int BetterNewNPC(this Entity entity, Vector2 spawn, int type, IEntitySource source = null, int initialSpawnSlot = 0, float ai0 = 0f, float ai1 = 0f, float ai2 = 0f, float ai3 = 0f, int target = 255, Vector2 velocity = default)
        {
            return entity.BetterNewNPC(spawn.X, spawn.Y, type, source, initialSpawnSlot, ai0, ai1, ai2, ai3, target, velocity);
        }
    }
}
