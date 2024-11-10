namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthDirt : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;

            DustType = DustID.Dirt;

            AddMapEntry(new Color(75, 32, 51));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
