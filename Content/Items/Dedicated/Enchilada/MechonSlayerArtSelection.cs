using CalamityMod.Cooldowns;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;

namespace TwilightEgress.Content.Items.Dedicated.Enchilada
{
    public class MechonSlayerArtSelection : CooldownHandler
    {
        public static new string ID => "MechonSlayerArtSelection";

        public override bool ShouldDisplay => true;

        public override bool ShouldPlayEndSound => true;

        public override LocalizedText DisplayName => Language.GetText($"Mods.TwilightEgress.Items.Dedicated.MechonSlayer.Cooldowns.{ID}");

        public override string Texture => "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/MechonSlayerArtSelection";

        public override string OutlineTexture => "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/MechonSlayerArtSelectionOutline";

        public override string OverlayTexture => "TwilightEgress/Assets/Textures/Items/Dedicated/Enchilada/MechonSlayerArtSelectionOverlay";

        public override Color OutlineColor => Color.DarkBlue;

        public override Color CooldownStartColor => Color.Lerp(Color.LightSkyBlue, Color.Cyan, 1f - instance.Completion);

        public override Color CooldownEndColor => CooldownStartColor;

        public override SoundStyle? EndSound => SoundID.DD2_DarkMageCastHeal;
    }
}
