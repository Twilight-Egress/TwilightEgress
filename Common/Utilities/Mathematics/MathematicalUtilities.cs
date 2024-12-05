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
        /// Simple pythagorean check for if ur inside a radius.
        /// </summary>
        /// <returns>True if the coord is within radius of the origin, false if it isn't. </returns>
        public static bool InRadius(Vector2 coords, Vector2 origin, float radius)
        {
            Vector2 position = coords - origin;
            return Pow(position.X, 2) + Pow(position.Y, 2) <= Pow(radius, 2);
        }

        /// <summary>
        /// Get random number from a vector position.
        /// </summary>
        public static float RandomFromVector(Vector2 input)
        {
            return (Sin(Vector2.Dot(input, new(12.9898f, 78.233f))) * 43758.5453f) % 1;
        }

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
    }
}
