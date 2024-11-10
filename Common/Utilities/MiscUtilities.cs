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
    }
}
