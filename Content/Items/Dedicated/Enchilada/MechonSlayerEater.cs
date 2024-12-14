using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace TwilightEgress.Content.Items.Dedicated.Enchilada
{
    public class MechonSlayerEater : CooldownHandler
    {
        public static new string ID => "MechonSlayerEater";

        public override bool ShouldDisplay => true;

        public override bool ShouldPlayEndSound => true;

        public override LocalizedText DisplayName => Language.GetText($"Mods.TwilightEgress.UI.Cooldowns.{ID}");

        public override string Texture => "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/MechonSlayerEater";

        public override string OutlineTexture => "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/MechonSlayerArtSelectionOutline";

        public override string OverlayTexture => "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/MechonSlayerArtSelectionOutline";

        public override Color OutlineColor => Color.LightSlateGray;

        public override Color CooldownStartColor => Color.Lerp(Color.SkyBlue, Color.DarkGray, 1f - instance.Completion);

        public override Color CooldownEndColor => CooldownStartColor;

        public override SoundStyle? EndSound => SoundID.DD2_DarkMageAttack;
    }
}
