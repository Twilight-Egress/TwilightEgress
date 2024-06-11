namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Very simple mathematical function. Calculates the percentage of the value of an integer.
        /// </summary>
        /// <returns>A value representing the specified percentage of the integer's original value. </returns>
        public static int GetPercentageOfInteger(this int integer, float percentage) => (int)(integer * percentage);

        /// <summary>
        /// Creates random, jagged <see cref="Vector2"/> points along the distance bewteen the source and destination of a line, akin to those of a lightning bolt.
        /// </summary>
        /// <param name="source">The starting point of the bolt.</param>
        /// <param name="destination">The end point of the bolt.</param>
        /// <param name="sway">The amount of variance in the displacement of points.</param>
        /// <param name="jaggednessNumerator">Controls how jagged the bolt appears. Higher values result in 
        /// less jaggedness, where as lower values result in more. Defaults to 1.</param>
        /// <returns>A list of <see cref="Vector2"/> points along the distance between the source and destination.</returns>
        public static List<Vector2> CreateLightningBoltPoints(Vector2 source, Vector2 destination, float sway = 80f, float jaggednessNumerator = 1f)
        {
            List<Vector2> results = new List<Vector2>();
            Vector2 tangent = destination - source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 8; i++)
                positions.Add(Main.rand.NextFloat());

            positions.Sort();

            float Jaggedness = jaggednessNumerator / sway;

            Vector2 prevPoint = source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Main.rand.NextFloat(-sway, sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = source + pos * tangent + displacement * normal;
                results.Add(point);
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(prevPoint);
            results.Add(destination);
            results.Insert(0, source);

            return results;
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
    }
}
