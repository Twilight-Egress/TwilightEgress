using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content.Sources;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core.Players.BuffHandlers;
using TwilightEgress.Core.Sources;

namespace TwilightEgress
{
    public partial class TwilightEgress : Mod
    {
        internal static Mod CalamityMod;

        internal static Texture2D Pixel;

        internal static TwilightEgress Instance { get; private set; }

        public override void Load()
        {
            Instance = this;
            CalamityMod = null;
            ModLoader.TryGetMod("CalamityMod", out CalamityMod);

            // TwilightEgress-specific loading.
            LoadLists();

            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                Pixel = new Texture2D(Main.graphics.GraphicsDevice, 1, 1);
                Pixel.SetData<Color>([Color.White]);
            });
        }

        public override void Unload()
        {
            Instance = null;
            CalamityMod = null;
            MusicDisplay = null;
            UnloadLists();
            BuffHandler.StuffToUnload();

            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                Pixel.Dispose();
                Pixel = null;
            });
        }

        public override IContentSource CreateDefaultContentSource()
        {
            RedirectContentSource source = new RedirectContentSource(base.CreateDefaultContentSource());

            source.AddRedirect("Content", "Assets/Textures");
            return source;
        }
    }
}