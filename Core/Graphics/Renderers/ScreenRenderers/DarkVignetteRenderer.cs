using Luminance.Core.Graphics;

namespace TwilightEgress.Core.Graphics.Renderers.ScreenRenderers
{
    public class DarkVignetteRenderer : SmartRenderer
    {
        private static int VignetteTime;

        private static int VignetteLifespan;

        private static float VignettePower;

        private static float VignetteBrightness;

        private static Vector2 VignettePosition;

        private static bool VignetteIsActive;

        private static bool CanDrawVignette => VignettePower > 0 || VignetteBrightness > 0;

        public static void ApplyDarkVignette(Vector2 vignettePosition, float vignettePower, float vignetteBrightness, int vignetteLifespan)
        {
            VignettePosition = vignettePosition;
            VignettePower = vignettePower;
            VignetteBrightness = vignetteBrightness;
            VignetteLifespan = vignetteLifespan;

            VignetteTime = 0;
            VignetteIsActive = true;
        }

        public override bool ShouldDrawRenderer => CanDrawVignette;

        public override SmartRendererDrawLayer DrawLayer => SmartRendererDrawLayer.BeforeFilters;

        public override void PostUpdate()
        {
            if (VignetteIsActive)
            {
                VignetteTime++;
                if (VignetteTime >= VignetteLifespan)
                {
                    VignetteTime = 0;
                    VignetteIsActive = false;
                }
            }
            else
            {
                if (VignetteBrightness > 0)
                {
                    VignettePower = Clamp(VignettePower + 0.03f, 0f, 20f);
                    VignetteBrightness = Clamp(VignetteBrightness - 0.01f, 0f, 50f);
                }
                else
                {
                    VignetteBrightness = 0;
                    VignettePower = 0;
                    VignettePosition = Vector2.Zero;
                }
            }
        }

        public override void DrawTarget(SpriteBatch spriteBatch)
        {
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            //var shader = Utilities.TryGetScreenShader("EllipticalVignetteShader");
            ManagedScreenFilter shader = ShaderManager.GetFilter("TwilightEgress.EllipticalVignette");
            shader.TrySetParameter("vignettePower", VignettePower);
            shader.TrySetParameter("vignetteBrightness", VignetteBrightness);
            shader.Apply();

            // This will extend the effect an extra 500 tiles over the screen.
            // The effect won't fit on the screen properly without this.
            Rectangle screenFit = new(-500, -500, Main.screenWidth + 600, Main.screenHeight + 600);

            Vector2 drawPosition = VignettePosition - Main.screenPosition;
            Vector2 origin = VignettePosition + new Vector2(500f) - Main.screenPosition;
            Main.spriteBatch.Draw(MainTarget.Target, drawPosition, screenFit, Color.White, 0f, origin, 1f, SpriteEffects.None, 0f);
            Main.spriteBatch.End();
        }
    }
}
