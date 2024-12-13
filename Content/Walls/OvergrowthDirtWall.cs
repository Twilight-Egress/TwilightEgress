using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Walls
{
    public class OvergrowthDirtWall : ModWall
    {
        public override void SetStaticDefaults()
        {
            DustType = DustID.Dirt;
            AddMapEntry(new Color(24, 10, 19));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
