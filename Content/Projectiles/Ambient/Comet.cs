using CalamityMod;
using Luminance.Common.Utilities;
using Luminance.Core.Graphics;
using TwilightEgress.Core.Graphics;

namespace TwilightEgress.Content.Projectiles.Ambient
{
    public class Comet : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public new string LocalizationCategory => "Projectiles.Ambient";

        public override string Texture => "TwilightEgress/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneAsteroidSmall";

        public override string GlowTexture => "TwilightEgress/Content/NPCs/CosmostoneShowers/Asteroids/CosmostoneAsteroidSmall_Glowmask";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Comet");
            Main.projFrames[Projectile.type] = 3;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 16;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.alpha = 255;
            Projectile.timeLeft = 1800;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            // We do a little bit of trolling :)))))
            Projectile.hostile = Main.zenithWorld;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            if (Projectile.alpha > 0)
                Projectile.alpha -= 20;

            Projectile.rotation += Projectile.velocity.X * 0.02f;

            if (Projectile.localAI[0] == 0f)
            {
                Projectile.localAI[0] = 1f;
                Projectile.frame = Main.rand.Next(0, 3);
                Projectile.netUpdate = true;
            }

            Lighting.AddLight(Projectile.Center, Color.SkyBlue.ToVector3());
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 15; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.BlueFairy, speed * 5f);
                d.noGravity = true;

                Dust d2 = Dust.NewDustPerfect(Projectile.Center, DustID.TintableDust, speed * 5f);
                d2.color = Color.Lerp(Color.Brown, Color.SaddleBrown, Main.rand.NextFloat());
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.DrawBackglow(Projectile.GetAlpha(Color.Cyan * 0.45f), 2f);
            //Projectile.DrawTextureOnProjectile(Projectile.GetAlpha(Color.White), Projectile.rotation, Projectile.scale, animated: true);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            int individualFrameHeight = texture.Height / Main.projFrames[Projectile.type];
            int currentYFrame = individualFrameHeight * Projectile.frame;
            Rectangle rectangle = new Rectangle(0, currentYFrame, texture.Width, individualFrameHeight);

            Vector2 origin = rectangle.Size() / 2f;
            DrawCosmostone(Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            return false;
        }

        public void DrawCosmostone(Vector2 position, Rectangle? sourceRectangle, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float worthless = 0f)
        {

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowmask = TextureAssets.GlowMask[Projectile.type].Value;

            Main.EntitySpriteDraw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);

            Main.spriteBatch.PrepareForShaders();

            ManagedShader shader = ShaderManager.GetShader("TwilightEgress.ManaPaletteShader");
            shader.TrySetParameter("flowCompactness", 3.0f);
            shader.TrySetParameter("gradientPrecision", 10f);
            shader.TrySetParameter("palette", TwilightEgressUtilities.CosmostonePalette);
            shader.Apply();
            Main.spriteBatch.Draw(glowmask, position, sourceRectangle, color, rotation, origin, scale, effects, worthless);
            Main.spriteBatch.ResetToDefault();
        }


        public float TrailWidthFunction(float trailLengthInterpolant) => 32f * Utils.GetLerpValue(0.75f, 0f, trailLengthInterpolant, true) * Projectile.scale * Projectile.Opacity;

        public Color TrailColorFunction(float trailLengthInterpolant) => Color.Lerp(Color.SkyBlue, Color.DeepSkyBlue, trailLengthInterpolant) * Projectile.Opacity;

        public Vector2 TrailOffsetFunction(float trailLengthInterpolant) => Projectile.Size * 0.5f;

        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            ShaderManager.TryGetShader("TwilightEgress.SmoothTextureMapTrail", out ManagedShader smoothTrail);
            smoothTrail.SetTexture(TextureAssets.Extra[ExtrasID.FlameLashTrailShape], 1);
            smoothTrail.TrySetParameter("time", Main.GlobalTimeWrappedHourly * 2.5f);

            PrimitiveSettings settings = new(TrailWidthFunction, TrailColorFunction, TrailOffsetFunction, true, true, smoothTrail);
            PrimitiveRenderer.RenderTrail(Projectile.oldPos, settings, 24);
        }
    }
}
