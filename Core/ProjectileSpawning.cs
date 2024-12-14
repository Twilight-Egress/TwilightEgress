using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace TwilightEgress.Core
{
    public static class ProjectileSpawning
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
        public static int BetterNewProjectile(this Entity entity, float spawnX, float spawnY, float velocityX, float velocityY, int type, int damage, float knockback, SoundStyle? sound = null, IEntitySource source = null, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0, bool damageChanges = false)
        {
            if (owner == -1)
                owner = Main.myPlayer;

            if (sound.HasValue)
                SoundEngine.PlaySound(sound, entity.Center);

            if (damageChanges)
            {
                float damageCorrection = 0.5f;
                if (Main.expertMode)
                    damageCorrection = 0.25f;
                if (Main.masterMode)
                    damageCorrection = 0.1667f;
                damage = (int)(damage * damageCorrection);
            }

            int index = Projectile.NewProjectile(source ?? entity.GetSource_FromAI(), spawnX, spawnY, velocityX, velocityY, type, damage, knockback, owner, ai0, ai1, ai2);
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
        public static int BetterNewProjectile(this Entity entity, Vector2 center, Vector2 velocity, int type, int damage, float knockback, SoundStyle? sound = null, IEntitySource source = null, int owner = -1, float ai0 = 0, float ai1 = 0, float ai2 = 0, bool damageChanges = false)
        {
            return entity.BetterNewProjectile(center.X, center.Y, velocity.X, velocity.Y, type, damage, knockback, sound, source, owner, ai0, ai1, ai2, damageChanges);
        }
    }
}
