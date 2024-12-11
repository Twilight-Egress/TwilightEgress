namespace TwilightEgress.Core
{
    public class LightningPointsBuilder
    {
        private Vector2 Source;
        private Vector2 Destination;
        private float Sway;
        private float Jaggedness;

        public LightningPointsBuilder() 
        {
            
        }

        /// <summary>
        /// Set the starting point of the lightning bolt.
        /// </summary>
        public LightningPointsBuilder SetSource(Vector2 source)
        {
            Source = source;
            return this;
        }

        /// <summary>
        /// Set the end point of the lightning bolt.
        /// </summary>
        public LightningPointsBuilder SetDestination(Vector2 destination)
        {
            Destination = destination; 
            return this;
        }

        /// <summary>
        /// Set the amount of variance in the displacement of points.
        /// </summary>
        public LightningPointsBuilder SetSway(float sway)
        {
            Sway = sway;
            return this;
        }

        /// <summary>
        /// Set how jagged the bolt appears. Higher values result in less jaggedness, where as lower values result in more.
        /// </summary>
        public LightningPointsBuilder SetJaggedness(float jaggedness)
        {
            Jaggedness = jaggedness;
            return this;
        }

        /// <summary>
        /// Creates random, jagged <see cref="Vector2"/> points along the distance bewteen the source and destination of a line, akin to those of a lightning bolt.
        /// </summary>
        /// <returns>A list of <see cref="Vector2"/> points along the distance between the source and destination.</returns>
        public List<Vector2> Create()
        {
            List<Vector2> results = new List<Vector2>();
            Vector2 tangent = Destination - Source;
            Vector2 normal = Vector2.Normalize(new Vector2(tangent.Y, -tangent.X));
            float length = tangent.Length();

            List<float> positions = new List<float>();
            positions.Add(0);

            for (int i = 0; i < length / 8; i++)
                positions.Add(Main.rand.NextFloat());

            positions.Sort();

            float jaggedness = Jaggedness / Sway;

            Vector2 prevPoint = Source;
            float prevDisplacement = 0;
            for (int i = 1; i < positions.Count; i++)
            {
                float pos = positions[i];

                // used to prevent sharp angles by ensuring very close positions also have small perpendicular variation.
                float scale = (length * Jaggedness) * (pos - positions[i - 1]);

                // defines an envelope. Points near the middle of the bolt can be further from the central line.
                float envelope = pos > 0.95f ? 20 * (1 - pos) : 1;

                float displacement = Main.rand.NextFloat(-Sway, Sway);
                displacement -= (displacement - prevDisplacement) * (1 - scale);
                displacement *= envelope;

                Vector2 point = Source + pos * tangent + displacement * normal;
                results.Add(point);
                prevPoint = point;
                prevDisplacement = displacement;
            }

            results.Add(prevPoint);
            results.Add(Destination);
            results.Insert(0, Source);

            return results;
        }
    }
}
