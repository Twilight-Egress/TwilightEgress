using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace TwilightEgress.Content.Walls
{
    public class OvergrowthDirtWall : ModWall
    {
        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            DustType = DustID.Dirt;
            AddMapEntry(new Color(24, 10, 19));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
