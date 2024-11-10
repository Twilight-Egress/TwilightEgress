using CalamityMod.Cooldowns;

namespace TwilightEgress.Content.Cooldowns
{
    public class MechonSlayerEater : CooldownHandler
    {
        public static new string ID => "MechonSlayerEater";

        public override bool ShouldDisplay => true;

        public override bool ShouldPlayEndSound => true;

        public override LocalizedText DisplayName => Language.GetText($"Mods.TwilightEgress.UI.Cooldowns.{ID}");

        public override string Texture => "TwilightEgress/Content/Cooldowns/MechonSlayerEater";

        public override string OutlineTexture => "TwilightEgress/Content/Cooldowns/MechonSlayerArtSelectionOutline";

        public override string OverlayTexture => "TwilightEgress/Content/Cooldowns/MechonSlayerArtSelectionOutline";

        public override Color OutlineColor => Color.LightSlateGray;

        public override Color CooldownStartColor => Color.Lerp(Color.SkyBlue, Color.DarkGray, 1f - instance.Completion);

        public override Color CooldownEndColor => CooldownStartColor;

        public override SoundStyle? EndSound => SoundID.DD2_DarkMageAttack;
    }
}
