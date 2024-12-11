using TwilightEgress.Assets;
using TwilightEgress.Core.Graphics;

namespace TwilightEgress.Content.Items.Dedicated.Marv
{
    public class ElectricSkyBolt : ModProjectile, ILocalizedModType, IPixelatedPrimitiveRenderer
    {
        public List<Vector2> StrikePositions = [];

        public const float MaxTime = 45;

        public ref float Timer => ref Projectile.ai[0];

        public Vector2 StrikePosition { get; set; }

        public new string LocalizationCategory => "Projectiles.Magic";

        public override string Texture => "TwilightEgress/Assets/ExtraTextures/GreyscaleObjects/SoftStar";

        public override void SetStaticDefaults() => ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;

        public override void SetDefaults()
        {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Opacity = 1f;
            Projectile.penetrate = 1;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
        }

        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            // Initialization.
            if (Timer is 0f)
            {
                StrikePosition = Main.MouseWorld;
                Projectile.rotation = Main.rand.NextFloat(TwoPi);
            }

            if (Timer is 1f)
            {
                // Generate the positions for the lightning bolt.
                StrikePositions = TwilightEgressUtilities.CreateLightningBoltPoints(Projectile.Center, StrikePosition);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), StrikePosition, Vector2.Zero, ModContent.ProjectileType<ElectricSkyBoltExplosion>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                int numOfMist = Main.rand.Next(5, 10);
                for (int i = 0; i < numOfMist; i++)
                {
                    Vector2 mistVelocity = Vector2.UnitX.RotatedByRandom(TwoPi) * Main.rand.NextFloat(1f, 3f);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), StrikePosition, mistVelocity, ModContent.ProjectileType<ElectricSkyBoltMist>(), Projectile.damage, Projectile.knockBack, Owner: Projectile.owner);
                }

                bool correctPlayerName = owner.name == "Marv" || owner.name == "EmolgaLover";
                SoundStyle lightning = correctPlayerName ? AssetRegistry.Sounds.PokemonThunderbolt : CommonCalamitySounds.LightningSound;
                SoundEngine.PlaySound(lightning, StrikePosition);
                Projectile.netUpdate = true;
            }

            if (Timer <= 30f)
                Projectile.Opacity = Lerp(1f, 0f, Timer / 30f);
            if (Timer >= MaxTime)
                Projectile.Kill();
            Timer++;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 speed = Utils.RandomVector2(Main.rand, -1f, 1f);
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.YellowStarDust, speed * 5f);
                d.noGravity = true;
            }
        }

        
        public override bool PreDraw(ref Color lightColor)
        {
            DrawBloomFlare();
            DrawBloomFlare(true);

            return false;
        }

        public float BoltWidthFunction(float completionRatio) => 4f * Lerp(1f, 0f, Timer / MaxTime);

        public Color BoltColorFunction(float completionRatio) => Color.Lerp(Color.Yellow, Color.Goldenrod, completionRatio) * Projectile.Opacity;


        public void RenderPixelatedPrimitives(SpriteBatch spriteBatch)
        {
            PrimitiveSettings settings = new(BoltWidthFunction, BoltColorFunction, null, false, true);
            PrimitiveRenderer.RenderTrail(StrikePositions, settings, (int)(StrikePosition.Length()));
        }

        public void DrawBloomFlare(bool strikePosition = false)
        {
            Texture2D texture = MiscTexturesRegistry.BloomFlare.Value;
            Vector2 drawPosition = (strikePosition ? StrikePosition : Projectile.Center) - Main.screenPosition;

            Main.spriteBatch.UseBlendState(BlendState.Additive);
            Main.EntitySpriteDraw(texture, drawPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale / 5f, 0);
            Main.EntitySpriteDraw(texture, drawPosition, null, Color.White * Projectile.Opacity, Projectile.rotation, texture.Size() / 2f, Projectile.scale / 4f, 0);
            Main.spriteBatch.ResetToDefault();
        }
    }
}
