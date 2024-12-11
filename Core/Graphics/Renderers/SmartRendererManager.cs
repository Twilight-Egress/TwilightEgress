using CalamityMod;
using Luminance.Common.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace TwilightEgress.Core.Graphics.Renderers
{
    public class SmartRendererManager : ModSystem
    {
        public static List<SmartRenderer> SmartRenderers { get; private set; } = new();

        public override void Load()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Main.QueueMainThreadAction(() =>
            {
                On_Main.CheckMonoliths += DrawToSmartTargets;
                On_Main.DrawInfernoRings += DrawRenderers_AfterEverything;
                On_Main.DrawNPCs += DrawRenderers_AfterNPCs;
                On_Main.DrawProjectiles += DrawRenderers_AfterProjectiles;
                On_Main.DrawPlayers_AfterProjectiles += DrawRenderers_AfterPlayers;
                On_Main.DrawBackgroundBlackFill += DrawRenderers_BeforeTiles;
                On_FilterManager.EndCapture += DrawRenderers_BeforeFilters;
            });
        }

        public override void Unload()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            Main.QueueMainThreadAction(() =>
            {
                On_Main.CheckMonoliths -= DrawToSmartTargets;
                On_Main.DrawInfernoRings -= DrawRenderers_AfterEverything;
                On_Main.DrawNPCs -= DrawRenderers_AfterNPCs;
                On_Main.DrawProjectiles -= DrawRenderers_AfterProjectiles;
                On_Main.DrawPlayers_AfterProjectiles -= DrawRenderers_AfterPlayers;
                On_Main.DrawBackgroundBlackFill -= DrawRenderers_BeforeTiles;
                On_FilterManager.EndCapture -= DrawRenderers_BeforeFilters;
            });

            SmartRenderers.Clear();
        }

        public override void PreUpdateEntities()
        {
            foreach (SmartRenderer renderer in SmartRenderers)
                renderer.PreUpdate();
        }

        public override void PostUpdateEverything()
        {
            foreach (SmartRenderer renderer in SmartRenderers)
                renderer.PostUpdate();
        }

        private void DrawToSmartTargets(On_Main.orig_CheckMonoliths orig)
        {
            orig.Invoke();

            if (Main.gameMenu)
                return;

            foreach (SmartRenderer renderer in SmartRenderers)
            {
                if (!renderer.ShouldDrawRenderer || renderer.DrawLayer == SmartRendererDrawLayer.BeforeFilters)
                    return;

                renderer.MainTarget.SwapToRenderTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                renderer.DrawToTarget(Main.spriteBatch);
                Main.spriteBatch.End();
            }

            Main.graphics.GraphicsDevice.SetRenderTarget(null);
        }

        private void DrawRenderers_AfterEverything(On_Main.orig_DrawInfernoRings orig, Main self)
        {
            orig.Invoke(self);

            List<SmartRenderer> smartRenderers_DrawAfterEverything = SmartRenderers.Where(x => x.ShouldDrawRenderer && x.DrawLayer == SmartRendererDrawLayer.AfterEverything && !Main.gameMenu).ToList();

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (SmartRenderer renderer in smartRenderers_DrawAfterEverything)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
        }

        private void DrawRenderers_AfterNPCs(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            orig.Invoke(self, behindTiles);

            List<SmartRenderer> smartRenderers_DrawAfterNPCs = SmartRenderers.Where(x => x.ShouldDrawRenderer && x.DrawLayer == SmartRendererDrawLayer.AfterNPCs && !Main.gameMenu).ToList();

            // Don't draw anything if the NPC is drawn behind tiles or if the main menu is active.
            if (behindTiles)
                return;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);
            
            foreach (SmartRenderer renderer in smartRenderers_DrawAfterNPCs)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.ExitShaderRegion();
        }

        private void DrawRenderers_AfterProjectiles(On_Main.orig_DrawProjectiles orig, Main self)
        {
            orig.Invoke(self);

            List<SmartRenderer> smartRenderers_DrawAfterProjectiles = SmartRenderers.Where(x => x.ShouldDrawRenderer && x.DrawLayer == SmartRendererDrawLayer.AfterProjectiles && !Main.gameMenu).ToList();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (SmartRenderer renderer in smartRenderers_DrawAfterProjectiles)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
        }

        private void DrawRenderers_AfterPlayers(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
        {
            orig.Invoke(self);

            List<SmartRenderer> smartRenderers_DrawAfterPlayers = SmartRenderers.Where(x => x.ShouldDrawRenderer && x.DrawLayer == SmartRendererDrawLayer.AfterPlayers && !Main.gameMenu).ToList();

            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            foreach (SmartRenderer renderer in smartRenderers_DrawAfterPlayers)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
        }

        private void DrawRenderers_BeforeTiles(On_Main.orig_DrawBackgroundBlackFill orig, Main self)
        {
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer);

            List<SmartRenderer> smartRenderers_DrawBeforeTiles = SmartRenderers.Where(x => x.ShouldDrawRenderer && x.DrawLayer == SmartRendererDrawLayer.BeforeTiles && !Main.gameMenu).ToList();

            foreach (SmartRenderer renderer in smartRenderers_DrawBeforeTiles)
                renderer.DrawTarget(Main.spriteBatch);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            orig.Invoke(self);
        }

        private void DrawRenderers_BeforeFilters(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (Main.gameMenu)
                return;

            List<SmartRenderer> smartRenderers_DrawBeforeFilters = SmartRenderers.Where(x => x.ShouldDrawRenderer && x.DrawLayer == SmartRendererDrawLayer.BeforeFilters && !Main.gameMenu).ToList();

            foreach (SmartRenderer renderer in smartRenderers_DrawBeforeFilters)
            {                    
                // Draw the contents of the screen onto our target, then swap back to the screen target,
                // where our target is then drawn with the screen's contents.
                renderer.MainTarget.SwapToRenderTarget();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(screenTarget1, Vector2.Zero, Color.White);
                Main.spriteBatch.End();

                screenTarget1.SwapToRenderTarget();
                renderer.DrawTarget(Main.spriteBatch);
            }

            orig.Invoke(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }
    }
}
