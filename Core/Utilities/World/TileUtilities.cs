using System;
using Terraria;

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
    }
}
