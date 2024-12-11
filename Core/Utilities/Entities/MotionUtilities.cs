using System;

namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Moves an <see cref="Entity"/> towards a Vector2.
        /// </summary>
        /// <param name="targetPosition">The position or Vector2 of the position to move to.</param>
        /// <param name="moveSpeed">The maximum movement speed.</param>
        /// <param name="turnResistance">The turn resistance of the movement. This affects how quickly it takes the entity to adjust direction.
        /// Defaults to 10.</param>
        public static void SimpleMove(this Entity entity, Vector2 targetPosition, float moveSpeed, float turnResistance = 10f)
        {
            Vector2 move = targetPosition - entity.Center;
            float magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > moveSpeed)
            {
                move *= moveSpeed / magnitude;
            }
            move = (entity.velocity * turnResistance + move) / (turnResistance + 1f);
            magnitude = (float)Math.Sqrt(move.X * move.X + move.Y * move.Y);
            if (magnitude > moveSpeed)
            {
                move *= moveSpeed / magnitude;
            }
            entity.velocity = move;
        }
    }
}
