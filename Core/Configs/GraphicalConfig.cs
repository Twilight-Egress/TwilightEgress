using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TwilightEgress.Core.Configs
{
    public class GraphicalConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static GraphicalConfig Instance;

        [Header("Graphics")]
        [BackgroundColor(192, 54, 64, 192)]
        [Range(1, 5000)]
        [Slider]
        [DefaultValue(2500)]
        public int ParticleLimit { get; set; }


        [BackgroundColor(192, 54, 64, 192)]
        [Range(1, 1000)]
        [Slider]
        [DefaultValue(500)]
        public int SkyEntityLimit { get; set; }
    }
}
