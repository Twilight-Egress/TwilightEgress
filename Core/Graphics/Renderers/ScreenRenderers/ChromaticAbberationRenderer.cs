using Luminance.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace TwilightEgress.Core.Graphics.Renderers.ScreenRenderers
{
    public class ChromaticAbberationRenderer : SmartRenderer
    {
        private static int ChromaTime;

        private static int ChromaLifespan;

        private static float ChromaStrength;

        private static Vector2 ChromaPosition;

        private static bool ChromaIsActive;

        private static float ChromaLifespanRatio => ChromaTime / (float)ChromaLifespan;

        public static void ApplyChromaticAbberation(Vector2 chromaPosition, float chromaStrength, int chromaLifespan)
        {
            ChromaPosition = chromaPosition;
            ChromaStrength = chromaStrength;
            ChromaLifespan = chromaLifespan;

            ChromaTime = 0;
            ChromaIsActive = true;
        }

        public override bool ShouldDrawRenderer => ChromaIsActive;

        public override SmartRendererDrawLayer DrawLayer => SmartRendererDrawLayer.BeforeFilters;

        public override void PostUpdate()
        {
            if (ChromaIsActive)
            {
                ChromaTime++;
                if (ChromaTime >= ChromaLifespan)
                {
                    ChromaTime = 0;
                    ChromaStrength = 0f;
                    ChromaPosition = Vector2.Zero;
                    ChromaIsActive = false;
                }
            }
        }

        public override void DrawTarget(SpriteBatch spriteBatch)
        {           
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

            ManagedScreenFilter shader = ShaderManager.GetFilter("TwilightEgress.ChromaticAbberation");
            shader.TrySetParameter("distortionAmount", (1f - ChromaLifespanRatio) * ChromaStrength);
            shader.TrySetParameter("impactPosition", ChromaPosition - Main.screenPosition);
            shader.Apply();

            spriteBatch.Draw(MainTarget.Target, Vector2.Zero, Color.White);

            spriteBatch.End();
        }
    }
}
