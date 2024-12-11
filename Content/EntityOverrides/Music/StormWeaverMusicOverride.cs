using CalamityMod.NPCs.StormWeaver;
using Terraria;
using Terraria.ModLoader;
using TwilightEgress.Core.Configs;

namespace TwilightEgress.Content.EntityOverrides.Music
{
    internal class StormWeaverMusicOverride : ModSceneEffect
    {
        public override int Music => MusicLoader.GetMusicSlot(Mod, "Assets/Sounds/Music/SupercellRogue");

        public override bool IsSceneEffectActive(Player player) => TwilightEgress.CanOverrideMusic(ModContent.NPCType<StormWeaverHead>()) && AudioConfig.Instance.OverrideCalamityMusic;

        public override SceneEffectPriority Priority => (SceneEffectPriority)10;
    }
}
