using System;
using System.Collections.Generic;

namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Simple pythagorean check for if ur inside a radius.
        /// </summary>
        /// <returns>True if the coord is within radius of the origin, false if it isn't. </returns>
        public static bool InRadius(Vector2 coords, Vector2 origin, float radius)
        {
            Vector2 position = coords - origin;
            return Math.Pow(position.X, 2) + Math.Pow(position.Y, 2) <= Math.Pow(radius, 2);
        }

        /// <summary>
        /// Get random number from a vector position.
        /// </summary>
        public static float RandomFromVector(Vector2 input)
        {
            Random random = new Random(input.GetHashCode());
            return (float)random.NextDouble();
        }
    }
}
