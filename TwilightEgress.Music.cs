using Terraria;
using Terraria.ModLoader;

namespace TwilightEgress
{
    public partial class TwilightEgress
    {
        internal static Mod MusicDisplay;

        public static bool IsBossRush { get => CalamityMod != null ? (bool)CalamityMod.Call("GetDifficultyActive", "bossrush") : false; }

        public static bool CanOverrideMusic(int npcID) => !IsBossRush && NPC.AnyNPCs(npcID);

        void AddMusic(string path, string displayName, string author)
        {
            MusicDisplay?.Call("AddMusic", (short)MusicLoader.GetMusicSlot(this, path), displayName, author, DisplayName);
        }

        public override void PostSetupContent()
        {
            MusicDisplay = null;
            if (ModLoader.TryGetMod("MusicDisplay", out MusicDisplay))
            {
                AddMusic("Assets/Sounds/Music/SecondLaw", "Second Law", "Sidetracked");
                AddMusic("Assets/Sounds/Music/SupercellRogue", "Supercell Rogue", "Sidetracked");
                AddMusic("Assets/Sounds/Music/YourSilhouette", "Your Silhouette", "Sidetracked");
                AddMusic("Assets/Sounds/Music/CosmostoneShowers", "Comet Dust", "ENNWAY!");
                AddMusic("Assets/Sounds/Music/Overgrowth", "Just a Sprinkle of Mercury", "Sidetracked");
            }
        }
    }
}
