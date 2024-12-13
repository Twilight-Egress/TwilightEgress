using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ModLoader;
using TwilightEgress.Content.Events.CosmostoneShowers;
using TwilightEgress.Core.Systems.EventHandlerSystem;

namespace TwilightEgress.Content.Skies
{
    public class CosmostoneShowersSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override float GetWeight(Player player) => 0.8f;

        public override bool IsSceneEffectActive(Player player) => EventHandlerManager.SpecificEventIsActive<CosmostoneShowers>() && (player.ZoneOverworldHeight || player.ZoneSkyHeight);

        public override void SpecialVisuals(Player player, bool isActive) => player.ManageSpecialBiomeVisuals("TwilightEgress:CosmostoneShowers", isActive);
    }

    public class CosmostoneShowersSky : CustomSky
    {
        private bool isActive;

        private float FadeOpacity;

        public override float GetCloudAlpha() => (1f - FadeOpacity) * 0.3f + 0.7f;

        public override void Activate(Vector2 position, params object[] args) => isActive = true;

        public override void Deactivate(params object[] args) => isActive = false;

        public override void Reset() => isActive = false;

        public override bool IsActive() => isActive || FadeOpacity > 0f;

        public override void Update(GameTime gameTime)
        {
            if (isActive && FadeOpacity < 1f)
                FadeOpacity += 0.01f;
            else if (!isActive && FadeOpacity > 0f)
                FadeOpacity -= 0.01f;

            // Disable vanilla Terraria's stars in turn of making ours more abundant and noticeable.
            for (int i = 0; i < Main.maxStars; i++)
                Main.star[i] = new();
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            Matrix transformationMatrix = Main.BackgroundViewMatrix.TransformationMatrix;
            Vector3 matrixDirection = new(1f, Main.BackgroundViewMatrix.Effects.HasFlag(SpriteEffects.FlipVertically) ? -1f : 1f, 1f);
            transformationMatrix.Translation -= Main.BackgroundViewMatrix.ZoomMatrix.Translation * matrixDirection;

            // Render the background. 
            if (maxDepth >= float.MaxValue && minDepth < float.MaxValue)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, transformationMatrix);
                
                CosmostoneShowers.RenderBackground(spriteBatch, FadeOpacity);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, transformationMatrix);
            }
        }
    }
}
