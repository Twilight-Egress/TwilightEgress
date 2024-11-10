using CalamityMod.Items.Accessories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightEgress
{
    public partial class TwilightEgress
    {
        internal static Mod MusicDisplay;

        public static bool IsBossRush { get => CalamityMod != null ? (bool)CalamityMod.Call("GetDifficultyActive", "bossrush") : false; }

        public static bool CanOverrideMusic(int npcID, bool overrideBossRush = false) => (!IsBossRush || overrideBossRush) && NPC.AnyNPCs(npcID);

        public override void PostSetupContent()
        {
            MusicDisplay = null;
            if (ModLoader.TryGetMod("MusicDisplay", out MusicDisplay))
            {
                void AddMusic (string path, string displayName, string author)
                    => MusicDisplay?.Call("AddMusic", (short)MusicLoader.GetMusicSlot(this, path), displayName, author, DisplayName);

                AddMusic("Assets/Sounds/Music/SecondLaw", "Second Law", "Sidetracked");
                AddMusic("Assets/Sounds/Music/SupercellRogue", "Supercell Rogue", "Sidetracked");
                AddMusic("Assets/Sounds/Music/YourSilhouette", "Your Silhouette", "Sidetracked");
            }
        }
    }
}
