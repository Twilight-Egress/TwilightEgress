using TwilightEgress.Content.Tiles.EnchantedOvergrowth;

namespace TwilightEgress.Content.Biomes.EnchantedOvergrowth
{
    internal class EnchantedOvergrowthTileCount : ModSystem
    {
        public int overgrowthTileCount;

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            overgrowthTileCount = tileCounts[ModContent.TileType<OvergrowthDirt>()];
        }
    }
}
