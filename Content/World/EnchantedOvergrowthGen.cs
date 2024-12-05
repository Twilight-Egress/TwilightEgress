using ReLogic.Threading;
using Terraria.IO;
using Terraria.WorldBuilding;
using TwilightEgress.Content.Tiles.EnchantedOvergrowth;
using static TwilightEgress.TwilightEgressUtilities;

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

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Polluting the land with mana";

            // set the size and initial position of the biome
            int size = (int)(Main.maxTilesX * 0.065f);
            int overgrowthPosX = (int)((GenVars.snowOriginLeft + GenVars.snowOriginRight) * 0.5f);
            int overgrowthPosY = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f));
            bool onLeftSide = overgrowthPosX < (Main.maxTilesX * 0.5f);

            bool IceBiomeTilesNearby(int x, int y)
            {
                int halfSize = (int)(size * 0.5f);

                for (int i = x - halfSize; i < x + halfSize; i++)
                {
                    for (int j = y; j < y + halfSize; j++)
                    {
                        int[] iceTiles = [TileID.IceBlock, TileID.SnowBlock, TileID.BlueDungeonBrick, TileID.GreenDungeonBrick, TileID.PinkDungeonBrick];

                        if (Framing.GetTileSafely(i, j).HasTile && iceTiles.Contains(Framing.GetTileSafely(i, j).TileType))
                            return true;
                    }
                }

                return false;
            }

            // move it inwards from the ice biome
            for (int i = 0; i < 10000; i++)
            {
                bool tilesDetected = true;

                while (tilesDetected = IceBiomeTilesNearby(overgrowthPosX, overgrowthPosY))
                    overgrowthPosX += (onLeftSide ? 100 : -100);

                if (!tilesDetected)
                    break;
            }

            // move position down until hitting a solid tile
            while (!Framing.GetTileSafely(overgrowthPosX, overgrowthPosY).HasTile)
                overgrowthPosY += 1;

            overgrowthPosY -= (int)(size * 0.08f);

            // generate the biome with noise
            FastNoiseLite noise = new FastNoiseLite();
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
            noise.SetSeed(WorldGen.genRand.Next());

            FastParallel.For(20, Main.maxTilesX - 20, (from, to, _) =>
            {
                for (int i = from; i <= to; i++)
                {
                    for (int j = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f)) - 20; j <= Main.maxTilesY - 20; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);

                        // The worldgen on shadertoy:
                        // https://www.shadertoy.com/view/Xf3fDN

                        Vector2 position = new Vector2(i, j);
                        Vector2 origin = new Vector2(overgrowthPosX, overgrowthPosY);

                        float noiseVal = noise.GetNoise(position.X, position.Y) * 0.5f;

                        float angle = Atan2(position.X - origin.X, position.Y - origin.Y) + PI;
                        float angleDomainWarp = (180f + (10f * noiseVal));
                        float radiusNoise = noise.GetNoise(angle * angleDomainWarp, angle * angleDomainWarp) * 0.5f + 0.5f;

                        float paintDomainWarp = (6 + noiseVal * 0.2f);
                        float paintNoise = noise.GetNoise(position.X * paintDomainWarp, position.Y * paintDomainWarp) * 0.5f + 0.5f;
                        float dripNoise = noise.GetNoise(position.X * 0.5f * paintDomainWarp, 0) * 0.5f + 0.5f;

                        position.X -= size * 0.025f * Pow(paintNoise, 2);
                        position.Y -= size * (0.025f * Pow(paintNoise, 2) + 0.4f * Pow(dripNoise, 2));

                        bool validPlacePos = InRadius(position, origin, size * (0.3f + 0.15f * radiusNoise));

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
                        Tile tile = Framing.GetTileSafely(i, j);

                        if (tile.TileType != ModContent.TileType<OvergrowthDirt>())
                            continue;

                        AdjacencyData<Tile> tileData = GetAdjacentTiles(i, j);

                        // generate grass
                        if (tile.TileType == ModContent.TileType<OvergrowthDirt>() && !tileData.top.HasTile)
                            tile.TileType = (ushort)ModContent.TileType<OvergrowthGrass>();
                    }
                }
            });
        }
    }
}
