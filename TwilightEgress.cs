using CalamityMod.Items.Accessories;
using CalamityMod.Items.Placeables.FurnitureAuric;
using TwilightEgress.Core.Players.BuffHandlers;

namespace TwilightEgress
{
    public partial class TwilightEgress : Mod
    {
        internal static Mod CalamityMod;

        internal static TwilightEgress Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            CalamityMod = null;
            ModLoader.TryGetMod("CalamityMod", out CalamityMod);

            // TwilightEgress-specific loading.
            LoadLists();
        }

        public override void Unload()
        {
            Instance = null;
            CalamityMod = null;
            MusicDisplay = null;
            UnloadLists();
            BuffHandler.StuffToUnload();
        }
    }
}