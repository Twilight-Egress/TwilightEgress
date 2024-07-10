using Terraria.ModLoader;
using TwilightEgress.Content.UI.Dialogue.UIElements;
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

            Main.QueueMainThreadAction(() =>
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                // UIElement doesn't have a built in Load() method, so these are loaded and unloaded here manually.
                ArdienaTextboxPrimitives.RectangleVertexBuffer ??= new(Main.graphics.GraphicsDevice, VertexPosition2DColorTexture.VertexDeclaration2D, 40, BufferUsage.WriteOnly);
                ArdienaTextboxPrimitives.RectangleIndexBuffer ??= new(Main.graphics.GraphicsDevice, IndexElementSize.SixteenBits, 60, BufferUsage.WriteOnly);
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

                ArdienaTextboxPrimitives.RectangleVertexBuffer = null;
                ArdienaTextboxPrimitives.RectangleIndexBuffer = null;
            });
        }
    }
}