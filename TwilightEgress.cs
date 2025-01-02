using ReLogic.Content.Sources;
using Terraria.ModLoader;
using TwilightEgress.Core.Players.BuffHandlers;
using TwilightEgress.Core.Sources;

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

        public override IContentSource CreateDefaultContentSource()
        {
            RedirectContentSource source = new RedirectContentSource(base.CreateDefaultContentSource());

            source.AddRedirect("Content", "Assets/Textures");
            return source;
        }
    }
}