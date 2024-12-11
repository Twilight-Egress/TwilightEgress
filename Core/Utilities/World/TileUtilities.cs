using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TwilightEgress
{
    public static partial class TwilightEgressUtilities 
    {
        public struct AdjacencyData<T>
        {
            public T top;
            public T bottom;
            public T left;
            public T right;
            public T topleft;
            public T topright;
            public T bottomleft;
            public T bottomright;
        }

        /// <summary>
        /// Easily get the tiles that are adjacent to the defined coordinates.
        /// <param name="i">The x position to get.</param>
        /// <param name="j">The y position to get.</param>
        /// </summary>
        public static AdjacencyData<T> GetAdjacentTiles<T>(int i, int j, Func<Tile, T> action)
        {
            AdjacencyData<T> tileData = new AdjacencyData<T>();

            tileData.top = action(Framing.GetTileSafely(i, j - 1));
            tileData.bottom = action(Framing.GetTileSafely(i, j + 1));

            tileData.left = action(Framing.GetTileSafely(i - 1, j));
            tileData.right = action(Framing.GetTileSafely(i + 1, j));

            tileData.topleft = action(Framing.GetTileSafely(i - 1, j - 1));
            tileData.topright = action(Framing.GetTileSafely(i + 1, j - 1));

            tileData.bottomleft = action(Framing.GetTileSafely(i - 1, j - 1));
            tileData.bottomright = action(Framing.GetTileSafely(i + 1, j - 1));

            return tileData;
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
