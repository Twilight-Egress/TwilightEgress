using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TwilightEgress.Core.Configs
{
    public class AudioConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AudioConfig Instance;

        [Header("Audio")]
        [BackgroundColor(192, 54, 64, 192)]
        [DefaultValue(true)]
        public bool OverrideCalamityMusic { get; set; }
    }
}
