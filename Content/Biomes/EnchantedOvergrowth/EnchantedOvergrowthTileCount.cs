using System;
using Terraria.ModLoader;
using TwilightEgress.Content.Tiles.EnchantedOvergrowth;

namespace TwilightEgress.Content.Biomes.EnchantedOvergrowth
{
    internal class EnchantedOvergrowthTileCount : ModSystem
    {
        public int overgrowthTileCount;

        public int[] overgrowthTiles = 
        [
            ModContent.TileType<OvergrowthDirt>(),
            ModContent.TileType<OvergrowthGrass>(),
            ModContent.TileType<Manastone>()
        ];

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            foreach (int tileType in overgrowthTiles) 
            {
                overgrowthTileCount += tileCounts[tileType];
            }
        }
    }
}
