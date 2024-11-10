using CalamityMod.NPCs.Signus;
using TwilightEgress.Core.Configs;

namespace TwilightEgress.Content.EntityOverrides.Music
{
    internal class SignusMusicOverride : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/YourSilhouette");

        public override bool IsSceneEffectActive(Player player) => TwilightEgress.CanOverrideMusic(ModContent.NPCType<Signus>()) && AudioConfig.Instance.OverrideCalamityMusic;

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
