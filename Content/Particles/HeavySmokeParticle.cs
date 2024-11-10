namespace TwilightEgress.Content.Particles
{
    public class HeavySmokeParticle : CasParticle
    {
        public bool Glowing;

        public int FrameX;

        public float HueShift;

        public override string AtlasTextureName => "TwilightEgress.HeavySmoke.png";

        public override int FrameCount => 7;

        public override BlendState BlendState => Glowing ? BlendState.Additive : BlendState.AlphaBlend;

        public HeavySmokeParticle(Vector2 position, Vector2 velocity, Color drawColor, int lifetime, float scale, float opacity, float rotationSpeed = 0f, bool glowing = false, float hueshift = 0f)
        {
            Position = position;
            Velocity = velocity;
            DrawColor = drawColor;
            Lifetime = lifetime;
            Scale = new(scale);
            Opacity = opacity;
            RotationSpeed = rotationSpeed;
            Glowing = glowing;
            HueShift = hueshift;

            Rotation = Main.rand.NextFloat(Tau);
            FrameX = Main.rand.Next(FrameCount);
        }

        public override void Update()
        {
            if (LifetimeRatio < 0.2f)
                Scale *= 1.01f;
            else
                Scale *= 0.975f;

            // Shift hues accordingly.
            Vector3 hueSaturationLightness = new((Main.rgbToHsl(DrawColor).X + HueShift) % 1f, (Main.rgbToHsl(DrawColor).X + HueShift) % 1f, (Main.rgbToHsl(DrawColor).X + HueShift) % 1f);
            DrawColor = Main.hslToRgb(hueSaturationLightness);

            // Fade out, spin and slow down.
            Opacity *= 0.98f;
            Rotation += RotationSpeed * Velocity.X.DirectionalSign();
            Velocity *= 0.9f;

            Opacity = Utils.GetLerpValue(1f, 0.85f, LifetimeRatio, true);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int frameYFromTime = (int)Math.Floor(Time / (Lifetime / 6f));
            Rectangle frame = new(FrameX * 80, frameYFromTime * 80, 80, 80);
            spriteBatch.Draw(Texture, Position - Main.screenPosition, frame, DrawColor * Opacity, Rotation, frame.Size() / 2f, Scale, Direction.ToSpriteDirection());
        }
    }
}
