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
            int lakesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lakes"));

            if (lakesIndex != -1)
            {
                tasks.Insert(lakesIndex + 1, new GenerateOvergrowthPass("Enchanted Overgrowth", 100f));
                tasks.Insert(lakesIndex + 2, new OvergrowthGrassPass("Overgrowth Grass", 101f));
            }
        }
    }

    public class GenerateOvergrowthPass : GenPass
    {
        public GenerateOvergrowthPass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        public bool Circle(Vector2 coords, Vector2 origin, float radius)
        {
            Vector2 position = coords - origin;
            return Pow(position.X, 2) + Pow(position.Y, 2) <= Pow(radius, 2);
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Polluting the land with mana";

            int size = (int)(Main.maxTilesX * 0.065f);
            int overgrowthPosX = (int)((GenVars.snowOriginLeft + GenVars.snowOriginRight) * 0.5f);
            int overgrowthPosY = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f));
            bool onLeftSide = overgrowthPosX < (Main.maxTilesX * 0.5f);

            bool IceBiomeTilesNearby(int x, int y)
            {
                for (int i = x - size; i < x + size; i++)
                {
                    for (int j = y; j < y + size; j++)
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
                    overgrowthPosX += (onLeftSide ? 100 : -100);

                if (!tilesDetected)
                    break;
            }

            /*while (Framing.GetTileSafely(overgrowthPosX, overgrowthPosY).HasTile)
                overgrowthPosY -= 1;*/

            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetSeed(WorldGen.genRand.Next());

            FastParallel.For(20, Main.maxTilesX - 20, (from, to, _) =>
            {
                for (int i = from; i <= to; i++)
                {
                    for (int j = 20; j <= Main.maxTilesY - 20; j++)
                    {
                        Tile tile = Main.tile[i, j];

                        float ratio = 800 / size;
                        float noiseVal = noise.GetNoise(i * ratio, j * ratio) * 0.25f + 0.5f;

                        bool validPlacePos = Circle(new Vector2(i, j) / size, new Vector2(overgrowthPosX, overgrowthPosY) / size, (0.5f + noiseVal));

                        if (tile.HasTile && validPlacePos)
                            tile.TileType = (ushort)ModContent.TileType<OvergrowthDirt>();
                    }
                }
            });

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
