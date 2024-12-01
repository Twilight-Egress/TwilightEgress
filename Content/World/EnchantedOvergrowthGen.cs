using ReLogic.Threading;
using Terraria.IO;
using Terraria.WorldBuilding;
using TwilightEgress.Content.Tiles.EnchantedOvergrowth;

namespace TwilightEgress.Content.World
{
    public class EnchantedOvergrowthGen : ModSystem
    {
        public static Point OvergrowthPos = Point.Zero;

        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            int dungeonIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));

            if (dungeonIndex != -1)
            {
                tasks.Insert(dungeonIndex + 1, new GenerateOvergrowthPass("Enchanted Overgrowth", 100f));
                tasks.Insert(dungeonIndex + 2, new OvergrowthGrassPass("Overgrowth Grass", 101f));
            }
        }
    }

    public class GenerateOvergrowthPass : GenPass
    {
        public GenerateOvergrowthPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Polluting the land with mana";

            int size = (int)(Main.maxTilesX * 0.0625f);
            int height = (int)(Main.maxTilesY * 0.1f);
            int overgrowthPosX = (int)((GenVars.snowOriginLeft + GenVars.snowOriginRight) * 0.5f);
            int overgrowthPosY = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f));
            bool onLeftSide = overgrowthPosX < (Main.maxTilesX * 0.5f);

            bool IceBiomeTilesNearby(int x, int y)
            {
                int checkArea = (int)(size * 0.5f);

                for (int i = x - checkArea; i < x + checkArea; i++)
                {
                    for (int j = y; j < y + checkArea; j++)
                    {
                        int[] iceTiles = [TileID.IceBlock, TileID.SnowBlock, TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick];

                        if (Main.tile[i, j].HasTile && iceTiles.Contains(Main.tile[i, j].TileType))
                            return true;
                    }
                }

                return false;
            }

            for (int i = 0; i < 10000; i++)
            {
                bool tilesDetected = true;

                while (tilesDetected = IceBiomeTilesNearby(overgrowthPosX, overgrowthPosY))
                    overgrowthPosX += (onLeftSide ? -100 : 100);

                if (!tilesDetected)
                    break;
            }


            for (int j = 0; j < height; j += 50)
            {
                // TO-DO: use noise as a function for whether or not to place a tile, and tilerunner to cut holes in that.
                // FastNoiseLite
                for (int cutOff = 0; cutOff < Main.maxTilesX / 50; cutOff += 50)
                    WorldGen.TileRunner(overgrowthPosX, overgrowthPosY + j + cutOff, size + j * 0.5f, 1, ModContent.TileType<OvergrowthDirt>(), false, 0f, 0f, true, true);
            }

            EnchantedOvergrowthGen.OvergrowthPos = new Point(overgrowthPosX, overgrowthPosY);
        }
    }

    public class OvergrowthGrassPass : GenPass
    {
        public OvergrowthGrassPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Polluting the land with mana";

            FastParallel.For(20, Main.maxTilesX - 20, (from, to, _) =>
            {
                for (int i = from; i <= to; i++)
                {
                    for (int j = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f)) - 20; j <= Main.maxTilesY - 20; j++)
                    {
                        Tile tile = Main.tile[i, j];
                        Tile tileAbove = Main.tile[i, j - 1];

                        if (tile.TileType == ModContent.TileType<OvergrowthDirt>() && !tileAbove.HasTile)
                            tile.TileType = (ushort)ModContent.TileType<OvergrowthGrass>();
                    }
                }
            });
        }
    }
}
