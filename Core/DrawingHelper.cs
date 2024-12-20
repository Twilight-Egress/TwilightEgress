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

        public static void DrawHitbox(this SpriteBatch spriteBatch, NPC npc, Color color)
        {
            Rectangle screenRectangle = new Rectangle(npc.getRect().X - (int)Main.screenPosition.X, npc.getRect().Y - (int)Main.screenPosition.Y, npc.getRect().Width, npc.getRect().Height);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screenRectangle, color);
        }

        public static void DrawHitbox(this SpriteBatch spriteBatch, Projectile projectile, Color color)
        {
            Rectangle screenRectangle = new Rectangle(projectile.getRect().X - (int)Main.screenPosition.X, projectile.getRect().Y - (int)Main.screenPosition.Y, projectile.getRect().Width, projectile.getRect().Height);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screenRectangle, color);
        }

        public static void DrawHitbox(this SpriteBatch spriteBatch, Player player, Color color)
        {
            Rectangle screenRectangle = new Rectangle(player.getRect().X - (int)Main.screenPosition.X, player.getRect().Y - (int)Main.screenPosition.Y, player.getRect().Width, player.getRect().Height);
            spriteBatch.Draw(TextureAssets.BlackTile.Value, screenRectangle, color);
        }
    }
}
