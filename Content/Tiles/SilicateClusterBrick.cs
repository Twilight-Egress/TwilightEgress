namespace TwilightEgress.Content.Tiles
{
    public class SilicateClusterBrick : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBrick[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;

            MineResist = 1.5f;
            DustType = DustID.Stone;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(82, 95, 192));
        }
    }
}
