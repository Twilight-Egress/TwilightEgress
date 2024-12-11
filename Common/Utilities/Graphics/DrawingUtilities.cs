namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities
    {
        /// <summary>
        /// The palette used for mana flowing through Cosmostone Asteroids.
        /// </summary>
        public static readonly Vector4[] CosmostonePalette =
        {
            new Color(96, 188, 246).ToVector4(),
            new Color(81, 158, 245).ToVector4(),
            new Color(76, 131, 242).ToVector4(),
            new Color(3, 96, 243).ToVector4(),
            new Color(48, 65, 197).ToVector4(),
            new Color(104, 94, 228).ToVector4(),
            new Color(157, 113, 239).ToVector4(),
        };

        public static void DrawTextureOnProjectile(this Projectile projectile, Color lightColor, float rotation, float scale, SpriteEffects spriteEffects = SpriteEffects.None, bool animated = false, Texture2D texture = null)
        {
            texture ??= TextureAssets.Projectile[projectile.type].Value;

            int individualFrameHeight = texture.Height / Main.projFrames[projectile.type];
            int currentYFrame = individualFrameHeight * projectile.frame;
            Rectangle rectangle = animated ?
                new Rectangle(0, currentYFrame, texture.Width, individualFrameHeight) :
                new Rectangle(0, 0, texture.Width, texture.Height);

            Vector2 origin = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), lightColor, rotation, origin, scale, spriteEffects, 0);
        }
    }
}

