using CalamityMod;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Assets;

namespace TwilightEgress.Content.Skies
{
    public class IceQueenSceneEffect : ModSceneEffect
    {
        public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;

        public override bool IsSceneEffectActive(Player player) => !Main.gameMenu && NPC.AnyNPCs(NPCID.IceQueen);

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("TwilightEgress:IceQueen", isActive);
        }
    }

    public class IceQueenSky : CustomSky
    {
        public class GlowStar
        {
            public int Time;

            public int Lifespan;

            public int IdenityIndex;

            public float Scale;

            public float Opacity;

            public float Depth;

            public Vector2 Position;

            public Vector2 Velocity;

            public Color Color;

            public GlowStar(int lifeSpan, int indenity, float scale, float depth, Vector2 position, Vector2 velocity, Color color)
            {
                Lifespan = lifeSpan;
                IdenityIndex = indenity;
                Scale = scale;
                Opacity = 1f;
                Depth = depth;
                Position = position;
                Velocity = velocity;
                Color = color;
            }
        }

        private Asset<Texture2D> GlowStarTexture;
        private Asset<Texture2D> PerlinTexture = AssetRegistry.Textures.PerlinNoise3;
        private Asset<Texture2D> CracksTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Cracks");

        private bool isActive;

        private float FadeOpacity;

        private List<GlowStar> GlowStars = new List<GlowStar>();

        private int GlowStarSpawnChance
        {
            get
            {
                if (!isActive)
                {
                    return int.MaxValue;
                }

                return 12;
            }
        }

        private float GlowStarSpeed
        {
            get
            {
                if (!isActive)
                {
                    return 0f;
                }

                return Main.rand.NextFloat(0.95f, 5f);
            }
        }

        private Color GetGlowStarColor()
        {
            Color firstColor = Utils.SelectRandom(Main.rand, Color.SkyBlue, Color.LightSkyBlue);
            Color secondColor = Utils.SelectRandom(Main.rand, Color.Cyan, Color.CornflowerBlue);
            return Color.Lerp(firstColor, secondColor, Main.rand.NextFloat(0.1f, 1f));
        }

        public override void OnLoad()
        {
            GlowStarTexture = AssetRegistry.Textures.SoftStar;
            PerlinTexture = AssetRegistry.Textures.PerlinNoise3;
            CracksTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/Cracks");
        }

        public override void Update(GameTime gameTime)
        {
            if (isActive && FadeOpacity < 1f)
            {
                FadeOpacity += 0.01f;
            }
            else if (!isActive && FadeOpacity > 0f)
            {
                FadeOpacity -= 0.01f;
            }

            if (isActive)
            {
                if (Main.rand.NextBool(GlowStarSpawnChance))
                {
                    int lifeSpan = Main.rand.Next(420, 480);
                    float depth = Main.rand.NextFloat(0f, 1f);
                    float startingScale = Main.rand.NextFloat(0.35f, 0.85f);
                    Vector2 startingPosition = Main.screenPosition + new Vector2(Main.screenWidth * Main.rand.NextFloat(-0.1f, 1.1f), Main.screenHeight * 1.05f);
                    Vector2 initialVelocity = -Vector2.UnitY.SafeNormalize(Vector2.Zero) * GlowStarSpeed;
                    GlowStars.Add(new GlowStar(lifeSpan, GlowStars.Count(), startingScale, depth, startingPosition, initialVelocity, GetGlowStarColor()));
                }
            }

            for (int i = 0; i < GlowStars.Count; i++)
            {
                GlowStars[i].Time++;
                GlowStar star = GlowStars[i];
                star.Position += star.Velocity;
                if (star.Time >= star.Lifespan)
                    star.Opacity -= 0.025f;
                star.Color *= star.Opacity;
            }
            GlowStars.RemoveAll(star => star.Opacity <= 0f);
        }

        public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
        {
            CalamityUtils.EnterShaderRegion(Main.spriteBatch, BlendState.Additive);

            ManagedShader shader = ShaderManager.GetShader("TwilightEgress.NoisyVignette");
            shader.SetTexture(PerlinTexture, 1);
            shader.SetTexture(CracksTexture, 2);
            shader.TrySetParameter("time", Main.GlobalTimeWrappedHourly / 10f);
            shader.TrySetParameter("scrollSpeed", 0.2f);
            shader.TrySetParameter("vignettePower", 1.65f);
            shader.TrySetParameter("vignetteBrightness", 1.25f);
            shader.TrySetParameter("primaryColor", Color.DarkGray.ToVector4());
            shader.TrySetParameter("secondaryColor", Color.DarkGray.ToVector4());
            shader.Apply();

            Vector2 screenArea = new(Main.instance.GraphicsDevice.Viewport.Width, Main.instance.GraphicsDevice.Viewport.Height);
            Vector2 textureScale = screenArea / TextureAssets.BlackTile.Value.Size();
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screenArea * 0.5f, null, Color.Lerp(Color.White, Color.DarkGray, 1f) * FadeOpacity, 0f, TextureAssets.BlackTile.Value.Size() * 0.5f, textureScale, SpriteEffects.None, 0f);
            Main.spriteBatch.ResetToDefault();

            for (int i = 0; i < GlowStars.Count; i++)
            {
                CalamityUtils.SetBlendState(spriteBatch, BlendState.Additive);
                Vector2 drawPosition = GlowStars[i].Position - Main.screenPosition;
                spriteBatch.Draw(TextureAssets.Extra[49].Value, drawPosition, null, GlowStars[i].Color * FadeOpacity, 0f, GlowStarTexture.Size() / 2f, GlowStars[i].Scale / 2f, SpriteEffects.None, 0f);
                spriteBatch.Draw(AssetRegistry.Textures.SoftStar.Value, drawPosition, null, GlowStars[i].Color * FadeOpacity, 0f, GlowStarTexture.Size() / 2f, GlowStars[i].Scale / 12f, SpriteEffects.None, GlowStars[i].Depth);
                CalamityUtils.SetBlendState(spriteBatch, BlendState.AlphaBlend);
            }
        }

        public override float GetCloudAlpha()
        {
            return (1f - FadeOpacity) * 0.3f + 0.7f;
        }

        public override void Activate(Vector2 position, params object[] args)
        {
            isActive = true;
        }

        public override void Deactivate(params object[] args)
        {
            isActive = false;
        }

        public override void Reset()
        {
            isActive = false;
        }

        public override bool IsActive() => isActive || FadeOpacity > 0f;
    }
}
