using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace TwilightEgress.Core.Graphics
{
    public class InterpolatedColorBuilder
    {
        private List<Color> Colors;

        public InterpolatedColorBuilder()
        {
            Colors = new List<Color>();
        }

        /// <summary>
        /// Adds a color to the list of colors to be interpolated from
        /// </summary>
        public InterpolatedColorBuilder AddColor(Color color)
        {
            Colors.Add(color);

            return this;
        }

        /// <summary>
        /// Interpolates between an array of colors. See <see href="https://en.wikipedia.org/wiki/Normal_distribution">this page</see>
        /// to learn more about how this works.
        /// </summary>
        /// <param name="interpolant">The amount or progress of interpolation.</param>
        /// <returns>A <see cref="Color"/> instance that's the specified point in the gradient.</returns>
        public Color GetInterpolatedColor(double interpolant)
        {
            double r = 0.0, g = 0.0, b = 0.0, a = 0.0;
            double total = 0.0;
            double step = 1.0 / (Colors.Count - 1);
            double mu = 0.0;
            double sigma2 = 0.035;

            foreach (Color color in Colors)
            {
                total += Math.Exp(-(interpolant - mu) * (interpolant - mu) / (2.0 * sigma2)) / Math.Sqrt(2.0 * Math.PI * sigma2);
                mu += step;
            }

            mu = 0.0;
            foreach (Color color in Colors)
            {
                double percent = Math.Exp(-(interpolant - mu) * (interpolant - mu) / (2.0 * sigma2)) / Math.Sqrt(2.0 * Math.PI * sigma2);
                mu += step;

                r += color.R * percent / total;
                g += color.G * percent / total;
                b += color.B * percent / total;
                a += color.A * percent / total;
            }

            return new Color((int)r, (int)g, (int)b, (int)a);
        }
    }
}
