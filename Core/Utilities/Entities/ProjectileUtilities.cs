using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

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
        public static int BetterNewProjectile(this Projectile projectile, float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, SoundStyle? sound = null, IEntitySource source = null, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            int p = 0;
            if (owner == -1)
                owner = Main.myPlayer;

            if (sound.HasValue)
                SoundEngine.PlaySound(sound, projectile.Center);

            p = Projectile.NewProjectile(source ?? projectile.GetSource_FromAI(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1, ai2);
            if (Main.projectile.IndexInRange(p))
                Main.projectile[p].netUpdate = true;

            return p;
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
        public static int BetterNewProjectile(this Projectile projectile, Vector2 center, Vector2 velocity, int type, int damage, float knockback, SoundStyle? sound = null, IEntitySource source = null, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0)
        {
            return projectile.BetterNewProjectile(center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, sound, source, owner, ai0, ai1, ai2);
        }

        /// <summary>
        /// Manually updates <see cref="Projectile.frame"/> and <see cref="Projectile.frameCounter"/> to animate projectile spritesheets.
        /// This method ONLY works with purely vertical spritesheets. If you are creating a projectile that uses a spritesheet with horizontal frames, do not use this method. You will have to manually
        /// write code to animate that.
        /// </summary>
        /// <param name="startFrame">The frame at which the animation starts. The projectile's frame will automatically be set to this if it
        /// is currently not on it.</param>
        /// <param name="endFrame">The frame at which the animation ends.</param>
        /// <param name="frameSpeed">How many frames it takes for <see cref="Projectile.frame"/> to increment.</param>
        public static void UpdateProjectileAnimationFrames(this Projectile projectile, int startFrame, int endFrame, int frameSpeed)
        {
            projectile.frameCounter++;
            if (projectile.frameCounter >= frameSpeed)
            {
                projectile.frame++;
                if (projectile.frame > endFrame)
                    projectile.frame = startFrame;
                projectile.frameCounter = 0;
            }
        }

        /// <summary>
        /// Adjusts a projectile's hitbox according to the input width, height and their current scale.
        /// </summary>
        /// <param name="initialWidth">The width you'd like the projectile to adjust to.</param>
        /// <param name="initialHeight">The height you'd like the projectile to adjust to.</param>
        public static void AdjustProjectileHitboxByScale(this Projectile projectile, float adjustedWidth, float adjustedHeight)
        {
            int oldWidth = projectile.width;
            int idealWidth = (int)(projectile.scale * adjustedWidth);
            int idealHeight = (int)(projectile.scale * adjustedHeight);
            if (oldWidth != idealWidth)
            {
                projectile.position.X += projectile.width / 2;
                projectile.position.Y += projectile.height / 2;
                projectile.width = idealWidth;
                projectile.height = idealHeight;
                projectile.position.X -= projectile.width / 2;
                projectile.position.Y -= projectile.height / 2;
            }
        }

        /// <summary>
        /// Gets the closest targettable NPC to a projectile.
        /// </summary>
        /// <param name="maxSearchDistance">The maximum distance around projectile to allow searching for viable targets.</param>
        /// <param name="maxSearchDistanceThroughWalls">The maximum distance through walls around a projectile to allow searching for viable targets.</param>
        /// <param name="foundTarget">Whether or not a target has been found. Must use the <see cref="out"/> keyword when initially calling the method.</param>
        /// <param name="target">The target that's been found. Must use the <see cref="out"/> keyword when initially calling the method.</param>
        public static void GetNearestTarget(this Projectile projectile, float maxSearchDistance, float maxSearchDistanceThroughWalls, out bool foundTarget, out NPC target)
        {
            foundTarget = false;
            target = null;

            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float distanceBetween = Vector2.Distance(npc.Center, projectile.Center);
                        bool closestToProjectile = Vector2.Distance(projectile.Center, npc.Center) > distanceBetween;
                        bool inRangeOfProjectile = distanceBetween < maxSearchDistance;
                        bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                        bool closestTargetThroughWalls = distanceBetween < maxSearchDistanceThroughWalls;

                        if (((closestToProjectile && inRangeOfProjectile) || !foundTarget) || (lineOfSight || closestTargetThroughWalls))
                        {
                            target = npc;
                            foundTarget = true;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Has the same purpose as the <see cref="GetNearestTarget(Projectile, float, float, out bool, out NPC)"/> method, though this method has 
        /// additional code specifically tailored to minion projectiles. Therefore, it should only be used for those.
        /// </summary>
        /// <param name="owner">The player who spawned this projectile.</param>
        /// <param name="maxSearchDistance">The maximum distance around projectile to allow searching for viable targets.</param>
        /// <param name="maxSearchDistanceThroughWalls">The maximum distance through walls around a projectile to allow searching for viable targets.</param>
        /// <param name="foundTarget">Whether or not a target has been found. Must use the <see cref="out"/> keyword when initially calling the method.</param>
        /// <param name="target">The target that's been found. Must use the <see cref="out"/> keyword when initially calling the method.</param>
        public static NPC GetNearestMinionTarget(this Projectile projectile, Player owner, float maxSearchDistance, float maxSearchDistanceThroughWalls, out bool foundTarget)
        {
            foundTarget = false;

            // Ensures minions using this will still target accordingly if the minion is compatible with the right-click targeting feature.
            if (owner.HasMinionAttackTargetNPC)
            {
                NPC target = Main.npc[owner.MinionAttackTargetNPC];
                float distanceBetweem = Vector2.Distance(target.Center, projectile.Center);
                if (distanceBetweem < maxSearchDistance)
                {
                    foundTarget = true;
                    return target;
                }
            }

            // Usual targetting code.
            if (!foundTarget)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy())
                    {
                        float distanceBetween = Vector2.Distance(npc.Center, projectile.Center);
                        bool closestToProjectile = Vector2.Distance(projectile.Center, npc.Center) > distanceBetween;
                        bool inRangeOfProjectile = distanceBetween < maxSearchDistance;
                        bool lineOfSight = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                        bool closestTargetThroughWalls = distanceBetween < maxSearchDistanceThroughWalls;

                        if (((closestToProjectile && inRangeOfProjectile) || !foundTarget) || (lineOfSight || closestTargetThroughWalls))
                        {
                            foundTarget = true;
                            return npc;
                        }
                    }
                }
            }

            return null;
        }
    }
}
