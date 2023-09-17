﻿using System.Collections.Generic;

namespace Cascade
{
    public static partial class Utilities
    {
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

        // These were all taken from https://easings.net/.
        // You can go there to get a demonstration of how each easing function should look.

        public static float LinearEase(float completionValue) => completionValue;

        public static float SineEaseIn(float completionValue) => 1 - (float)Math.Cos(completionValue * Math.PI / 2D);

        public static float SineEaseOut(float completionValue) => (float)Math.Sin(completionValue * Math.PI / 2D);

        public static float SineEaseInOut(float completionValue) => (float)(-(Math.Cos(Math.PI * completionValue) - 1) / 2D);

        public static float CubicEaseIn(float completionValue) => (float)Math.Pow(completionValue, 3D);

        public static float CubicEaseOut(float completionValue) => (float)(1 - Math.Pow(1 - completionValue, 3D));

        public static float CubicEaseInOut(float completionValue) => completionValue < 0.5f ? 4f * (float)Math.Pow(completionValue, 3D) : (float)(1 - Math.Pow(-2D * completionValue + 2D, 3D) / 2D);

        public static float QuintEaseIn(float completionValue) => (float)Math.Pow(completionValue, 5D);

        public static float QuintEaseOut(float completionValue) => (float)(1 - Math.Pow(1 - completionValue, 5D));

        public static float QuintEaseInOut(float completionValue) => completionValue < 0.5f ? 16f * (float)Math.Pow(completionValue, 5D) : (float)(1 - Math.Pow(-2D * completionValue + 2D, 5D) / 2D);

        public static float CircEaseIn(float completionValue) => 1 - (float)Math.Sqrt(1 - Math.Pow(completionValue, 2D));

        public static float CircEaseOut(float completionValue) => (float)Math.Sqrt(1 - Math.Pow(completionValue - 1, 2D));

        public static float CircEaseInOut(float completionValue) => (float)(completionValue < 0.5 ? (1 - (float)Math.Sqrt(1 - Math.Pow(2D * completionValue, 2D))) / 2D : (float)(Math.Sqrt(1 - Math.Pow(-2D * completionValue + 2D, 2D)) + 1) / 2);

        public static float QuadEaseIn(float completionValue) => (float)Math.Pow(completionValue, 2D);

        public static float QuadEaseOut(float completionValue) => 1 - (float)Math.Pow(1 - completionValue, 2D);

        public static float QuadEaseInOut(float completionValue) => (float)(completionValue < 0.5 ? 2D * (float)Math.Pow(completionValue, 2D) : 1 - (float)Math.Pow(-2D * completionValue + 2D, 2D) / 2D);

        public static float QuartEaseIn(float completionValue) => (float)Math.Pow(completionValue, 4D);

        public static float QuartEaseOut(float completionValue) => 1 - (float)Math.Pow(1 - completionValue, 4D);

        public static float QuartEaseInOut(float completionValue) => (float)(completionValue < 0.5 ? 8 * (float)Math.Pow(completionValue, 4D) : 1 - (float)Math.Pow(-2D * completionValue + 2D, 4D) / 2D);

        public static float ExpoEaseIn(float completionValue) => completionValue == 0 ? 0 : (float)Math.Pow(2D, 10D * completionValue - 10D);

        public static float ExpoEaseOut(float completionValue) => completionValue == 1 ? 1 : 1 - (float)Math.Pow(2D, -10D * completionValue);

        public static float ExpoEaseInOut(float completionValue) => (float)(completionValue == 0 ? 0 : completionValue == 1 ? 1 : completionValue < 0.5 ? (float)Math.Pow(2D, 20 * completionValue - 10D) / 2D : 2D - (float)Math.Pow(2D, -20 * completionValue + 10D) / 2D);
    }
}
