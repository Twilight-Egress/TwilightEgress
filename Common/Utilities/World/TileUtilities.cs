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
        /// </summary>
        public static AdjacencyData<Tile> GetAdjacentTiles(int i, int j)
        {
            AdjacencyData<Tile> tileData = new AdjacencyData<Tile>();

            tileData.top = Main.tile[i, j - 1];
            tileData.bottom = Main.tile[i, j + 1];

            tileData.left = Main.tile[i - 1, j];
            tileData.right = Main.tile[i + 1, j];

            tileData.topleft = Main.tile[i - 1, j - 1];
            tileData.topright = Main.tile[i + 1, j - 1];

            tileData.bottomleft = Main.tile[i - 1, j - 1];
            tileData.bottomright = Main.tile[i + 1, j - 1];

            return tileData;
        }
    }
}
