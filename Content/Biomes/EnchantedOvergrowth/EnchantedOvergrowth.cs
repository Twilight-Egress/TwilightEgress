using System.Linq;
using System;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using TwilightEgress.Content.Tiles.EnchantedOvergrowth;

namespace TwilightEgress.Content.Biomes.EnchantedOvergrowth
{
    public class EnchantedOvergrowth : ModBiome, ILocalizedModType
    {
        public new string LocalizationCategory => "Biomes";

        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Overgrowth");

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        public override bool IsBiomeActive(Player player)
        {
            int[] tileTypes = [
                ModContent.TileType<OvergrowthDirt>(),
                ModContent.TileType<OvergrowthGrass>(),
                ModContent.TileType<Manastone>()
            ];
            int searchBoxSideLength = 40;
            float ratioNeeded = 0.1f;

            Point playerTilePos = player.Center.ToTileCoordinates();
            int halfBoxLength = (int)(searchBoxSideLength * 0.5);
            int numberOfTiles = 0;

            for (int i = -halfBoxLength; i <= halfBoxLength; i++)
            {
                for (int j = -halfBoxLength; j <= halfBoxLength; j++)
                {
                    if (tileTypes.Contains(Main.tile[playerTilePos.X + i, playerTilePos.Y + j].TileType))
                        numberOfTiles++;
                }
            }

            return (float)(numberOfTiles / Math.Pow(searchBoxSideLength, 2)) >= ratioNeeded;
        }

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
    }
}
