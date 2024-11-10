using Terraria.Graphics.Capture;

namespace TwilightEgress.Content.Biomes.EnchantedOvergrowth
{
    public class EnchantedOvergrowth : ModBiome
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/Overgrowth");

        public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Normal;

        public override bool IsBiomeActive(Player player) => ModContent.GetInstance<EnchantedOvergrowthTileCount>().overgrowthTileCount >= 100;

        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeLow;
    }
}
