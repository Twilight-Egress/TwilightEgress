namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// Coordinates for the center of the screen.
        /// </summary>
        public static Vector2 ScreenCenter => Main.screenPosition + new Vector2(Main.screenWidth / 2f, Main.screenHeight / 2f);

        /// <summary>
        /// Flips the sprite of an entity based on its <see cref="Entity.direction"/> field.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static SpriteEffects DirectionBasedSpriteEffects(this Entity entity)
            => entity.direction < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

        /// <summary>
        /// The palette used for mana flowing through Cosmostone Asteroids.
        /// </summary>
        public static readonly Vector4[] CosmostonePalette =
        {
            new Color(96, 188, 246).ToVector4(),
            new Color(81, 158, 245).ToVector4(),
            new Color(76, 131, 242).ToVector4(),
            new Color(3, 96, 243).ToVector4(),
            new Color(48, 65, 197).ToVector4(),
            new Color(104, 94, 228).ToVector4(),
            new Color(157, 113, 239).ToVector4(),
        };

        public static readonly Vector4[] CosmostoneSkyPaletteTwo =
        {
            new Color(255, 248, 111).ToVector4(),
            new Color(255, 237, 111).ToVector4(),
            new Color(255, 229, 111).ToVector4(),
            new Color(255, 218, 111).ToVector4(),
            new Color(255, 208, 111).ToVector4(),
            new Color(255, 200, 111).ToVector4(),
            new Color(255, 192, 111).ToVector4(),
        };

        /// <summary>
        /// Converts a hexidecimal code into RGB values.
        /// </summary>
        /// <param name="hexCode">The hexidecimal code of the color you'd like to convert.</param>
        /// <returns>A <see cref="Color"/> instance with the correct RGB values from the hex code.</returns>
        public static Color HexToRGB(string hexCode)
        {
            int color = Convert.ToInt32(hexCode, 16);

            int r = (color & 0xff0000) >> 16;
            int g = (color & 0xff00) >> 8;
            int b = (color & 0xff);

            return new Color(r, g, b);
        }

        /// <summary>
        /// Interpolates between an array of colors. See <see href="https://en.wikipedia.org/wiki/Normal_distribution">this page</see>
        /// to learn more about how this works.
        /// </summary>
        /// <param name="colors">The array of colors to interpolate between.</param>
        /// <param name="x">The amount or progress of interpolation.</param>
        /// <returns>A <see cref="Color"/> instance that's the specified point in the gradient.</returns>
        public static Color InterpolateColor(Color[] colors, double x)
        {
            double r = 0.0, g = 0.0, b = 0.0;
            double total = 0.0;
            double step = 1.0 / (colors.Length - 1);
            double mu = 0.0;
            double sigma2 = 0.035;

            foreach (Color color in colors)
            {
                total += Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma2)) / Math.Sqrt(2.0 * Math.PI * sigma2);
                mu += step;
            }

            mu = 0.0;
            foreach (Color color in colors)
            {
                double percent = Math.Exp(-(x - mu) * (x - mu) / (2.0 * sigma2)) / Math.Sqrt(2.0 * Math.PI * sigma2);
                mu += step;

                r += color.R * percent / total;
                g += color.G * percent / total;
                b += color.B * percent / total;
            }

            System.Drawing.Color newColor = System.Drawing.Color.FromArgb(255, (int)r, (int)g, (int)b);
            return new Color(newColor.R, newColor.G, newColor.B, newColor.A);
        }

        /// <summary>
        /// Prepares a <see cref="RasterizerState"/> with screen culling enabled. This is mainly used for improving performance when drawing.
        /// </summary>
        /// <returns><see cref="Main.Rasterizer"/> with ScissorTestEnable enabled and a ScissorRectangle that covers the width and height
        /// of the entire screen."/></returns>
        public static RasterizerState PrepareScissorRectangleState()
        {
            Main.Rasterizer.ScissorTestEnable = true;
            Main.instance.GraphicsDevice.ScissorRectangle = new(-5, -5, Main.screenWidth + 10, Main.screenHeight + 10);
            return Main.Rasterizer;
        }
    }
}

