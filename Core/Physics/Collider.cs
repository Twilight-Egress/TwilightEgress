using Microsoft.Xna.Framework;
using System;
using TwilightEgress.Core.Geometry;

namespace TwilightEgress.Core.Physics
{
    public class Collider
    {
        /// <summary>
        /// https://dyn4j.org/2010/01/sat/
        /// Separating axis theorem
        /// </summary>
        /// <param name="polygon1"></param>
        /// <param name="polygon1Position"></param>
        /// <param name="polygon2"></param>
        /// <param name="polygon2Position"></param>
        /// <returns></returns>
        public Vector2? SeparatingAxisTheorem(Polygon polygon1, Vector2 polygon1Position, Polygon polygon2, Vector2 polygon2Position)
        {
            float shortestDist = float.MaxValue;

            // Get the offset between the two shapes
            Vector2 vOffset = new Vector2(polygon1Position.X - polygon2Position.X, polygon1Position.Y - polygon2Position.Y);

            float distance = 0f;
            Vector2 vector = Vector2.Zero;

            // Loop over all of the sides on the first polygon and check the perpendicular axis
            for (int i = 0; i < polygon1.Vertices.Length; i++)
            {
                // Get the perpendicular axis that we will be projecting onto
                Vector2 axis = GetPerpendicularAxis(polygon1.Vertices, i);
                // Project each point onto the axis 
                (float, float) poly1Range = ProjectVerticesForMinMax(axis, polygon1.Vertices);
                (float, float) poly2Range = ProjectVerticesForMinMax(axis, polygon2.Vertices);

                // Shift the first polygons min max along the axis by the amount of offset between them
                float scalerOffset = Vector2.Dot(axis, vOffset);
                poly1Range.Item1 += scalerOffset;
                poly2Range.Item2 += scalerOffset;

                // Now check for a gap betwen the relative min's and max's
                if ((poly1Range.Item1 - poly2Range.Item2 > 0) || (poly2Range.Item1 - poly1Range.Item2 > 0))
                    return null;

                // Calc the separation and store if this is the shortest
                float distMin = (poly2Range.Item2 - poly1Range.Item1) * -1;

                // Check if this is the shortest by using the absolute val
                float distMinAbs = Math.Abs(distMin);
                if (distMinAbs < shortestDist)
                {
                    shortestDist = distMinAbs;

                    distance = distMin;
                    vector = axis;
                }
            }

            if (distance == 0f && vector == Vector2.Zero)
                return null;

            // Calc the final separation
            return new Vector2(vector.X * distance, vector.Y * distance);
        }

        /// <summary>
        /// Loops over all of the vertices in an array, projects them onto the given axis, and return the min / max range of all points
        /// </summary>
        /// <param name="axis"></param>
        /// <param name="vertices"></param>
        /// <returns></returns>
        public (float, float) ProjectVerticesForMinMax(Vector2 axis, Vector2[] vertices)
        {
            // Note that we project the first point to both min and max
            float minimum = Vector2.Dot(axis, vertices[0]);
            float maximum = minimum;

            // Now we loop over the remiaing vers, updating min/max as required
            for (int j = 1; j < vertices.Length; j++)
            {
                float temp = Vector2.Dot(axis, vertices[j]);
                if (temp < minimum) minimum = temp;
                if (temp > maximum) maximum = temp;
            }

            return (minimum, maximum);
        }

        // Small helper method that looks at the verts of the polygon and return the perpendicular axis of a particular side
        private Vector2 GetPerpendicularAxis(Vector2[] vertices, int index)
        {
            Vector2 point1 = vertices[index];
            Vector2 point2 = index >= vertices.Length - 1 ? vertices[0] : vertices[index + 1];  // Get the next index, or wrap around if at the end

            Vector2 axis = new Vector2(-(point2.Y - point1.Y), point2.X - point1.X);
            axis.Normalize();
            return axis;
        }
    }
}
