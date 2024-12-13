using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;

namespace TwilightEgress.Core
{
    public static class DrawingHelper
    {
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

        /// <summary>
        /// Easily draw tile textures using SpecialDraw 
        /// <param name="spriteBatch">The sprite batch being used.</param>
        /// <param name="texture">The texture to draw.</param>
        /// <param name="i">The x position to draw at.</param>
        /// <param name="j">The y position to draw at.</param>
        /// <param name="sourceRectangle">The source rectangle to draw.</param>
        /// <param name="paintColor">The paint color of the tile.</param>
        /// <param name="rotation">Orientation of the texture.</param>
        /// <param name="origin">The origin of the texture.</param>
        /// <param name="scale">Scale the texture is drawn at.</param>
        /// <param name="lighted">Whether the texture is affected by light.</param>
        /// </summary>
        public static void DrawTileTexture(this SpriteBatch spriteBatch, Texture2D texture, int i, int j, Rectangle sourceRectangle, Color paintColor, float rotation, Vector2 origin, float scale = 1f, bool lighted = true)
        {
            Vector2 drawPosition = new Vector2(i * 16, j * 16) - Main.screenPosition;

            if (!Main.drawToScreen)
                drawPosition += new Vector2(Main.offScreenRange);

            Color drawColor = lighted ? Lighting.GetColor(i, j) : Color.White;

            drawColor.R *= (byte)(paintColor.R / 255);
            drawColor.G *= (byte)(paintColor.G / 255);
            drawColor.B *= (byte)(paintColor.B / 255);

            spriteBatch.Draw(texture, drawPosition, sourceRectangle, drawColor, 0.0f, origin, scale, SpriteEffects.None, 0.0f);
        }
    }
}
