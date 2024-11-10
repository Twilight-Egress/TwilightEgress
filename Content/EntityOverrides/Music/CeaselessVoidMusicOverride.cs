using CalamityMod.NPCs.CeaselessVoid;
using TwilightEgress.Core.Configs;

namespace TwilightEgress.Content.EntityOverrides.Music
{
    internal class CeaselessVoidMusicOverride : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/SecondLaw");

        public override bool IsSceneEffectActive(Player player) => TwilightEgress.CanOverrideMusic(ModContent.NPCType<CeaselessVoid>()) && AudioConfig.Instance.OverrideCalamityMusic;

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
