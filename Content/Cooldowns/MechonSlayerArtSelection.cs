using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace TwilightEgress.Content.Cooldowns
{
    public class MechonSlayerArtSelection : CooldownHandler
    {
        public static new string ID => "MechonSlayerArtSelection";

        public override bool ShouldDisplay => true;

        public override bool ShouldPlayEndSound => true;

        public override LocalizedText DisplayName => Language.GetText($"Mods.TwilightEgress.UI.Cooldowns.{ID}");

        public override string Texture => "TwilightEgress/Content/Cooldowns/MechonSlayerArtSelection";

        public override Color OutlineColor => Color.DarkBlue;

        public override Color CooldownStartColor => Color.Lerp(Color.LightSkyBlue, Color.Cyan, 1f - instance.Completion);

        public override Color CooldownEndColor => CooldownStartColor;

        public override SoundStyle? EndSound => SoundID.DD2_DarkMageCastHeal;
    }
}
