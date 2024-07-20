namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        public static readonly string EmptyPixelPath = "TwilightEgress/Assets/ExtraTextures/EmptyPixel";

        public static readonly string PixelPath = "Luminance/Assets/Pixel";

        /// <summary>
        /// Shorthand check for <c>Main.netMode != NetmodeID.MultiplayerClient</c>. Typically used when spawning entities such as NPCs and Projectiles.
        /// This does not need to be called when using TwilightEgress's entity spawning utilities, such as <see cref="TwilightEgressUtilities.SpawnNPC(NPC, float, float, int, int, float, float, float, float, int, Vector2)"/>
        /// </summary>
        /// <returns></returns>
        public static bool ObligatoryNetmodeCheckForSpawningEntities() => Main.netMode != NetmodeID.MultiplayerClient;

        public static T RandomElementByWeight<T>(this IEnumerable<T> sequence, Func<T, float> weightSelector)
        {
            float totalWeight = sequence.Sum(weightSelector);
            // The weight we are after...
            float itemWeightIndex = (float)new Random().NextDouble() * totalWeight;
            float currentWeightIndex = 0;

            foreach (var item in from weightedItem in sequence select new { Value = weightedItem, Weight = weightSelector(weightedItem) })
            {
                currentWeightIndex += item.Weight;

                // If we've hit or passed the weight we are after for this item then it's the one we want....
                if (currentWeightIndex > itemWeightIndex)
                    return item.Value;

            }

            return default(T);
        }
        public static Texture2D FlipTexture2D(Texture2D input, bool vertical, bool horizontal)
        {
            Texture2D flipped = new Texture2D(input.GraphicsDevice, input.Width, input.Height);
            Color[] data = new Color[input.Width * input.Height];
            Color[] flipped_data = new Color[data.Length];

            input.GetData<Color>(data);

            for (int x = 0; x < input.Width; x++)
            {
                for (int y = 0; y < input.Height; y++)
                {
                    int index = 0;
                    if (horizontal && vertical)
                        index = input.Width - 1 - x + (input.Height - 1 - y) * input.Width;
                    else if (horizontal && !vertical)
                        index = input.Width - 1 - x + y * input.Width;
                    else if (!horizontal && vertical)
                        index = x + (input.Height - 1 - y) * input.Width;
                    else if (!horizontal && !vertical)
                        index = x + y * input.Width;

                    flipped_data[x + y * input.Width] = data[index];
                }
            }

            flipped.SetData<Color>(flipped_data);

            return flipped;
        }
    }
}
