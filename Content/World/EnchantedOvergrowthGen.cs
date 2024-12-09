﻿using ReLogic.Threading;
using Terraria.IO;
using Terraria.WorldBuilding;
using TwilightEgress.Content.Tiles.EnchantedOvergrowth;
using TwilightEgress.Content.Walls;
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
                tasks.Insert(lakesIndex + 2, new OvergrowthFoliagePass("Overgrowth Foliage", 101f));
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
            progress.Message = "Polluting the land with magic";

            // set the size and initial position of the biome
            int size = (int)(Main.maxTilesX * 0.065f);
            int overgrowthPosY = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f));

            List<int> xPositionsCanGenerateAt = new List<int>();

            // find x positions that aren't near anything
            for (int i = 20; i < Main.maxTilesX - 20; i += 100)
            {
                bool nearSpawn = Main.spawnTileX > i - size * 2 && Main.spawnTileX < i + size * 2;

                if (!nearSpawn && !TilesToAvoidNearby(size, i, overgrowthPosY))
                    xPositionsCanGenerateAt.Add(i);
            }

            int overgrowthPosX = WorldGen.genRand.Next(xPositionsCanGenerateAt);

            // move position down until hitting a solid tile
            while (!Framing.GetTileSafely(overgrowthPosX, overgrowthPosY).HasTile)
                overgrowthPosY += 1;

            overgrowthPosY -= (int)(size * 0.08f);

            // generate the biome with noise
            // to-do: use sobel operator or nearest neighbor to get rid of random dirt splotches
            // https://en.wikipedia.org/wiki/Sobel_operator
            // https://en.wikipedia.org/wiki/Nearest-neighbor_interpolation
            // also just redo how surface gen works lmao
            // how the fuck do i stop it from generating on a sky island
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

                        if (!InRadius(position, origin, size * (0.3f + 0.15f * radiusNoise)))
                            continue;

                        if (tile.WallType != 0)
                            tile.WallType = (ushort)ModContent.WallType<OvergrowthDirtWall>();

                        if (!tile.HasTile)
                            continue;

                        if (tile.TileType == TileID.Stone)
                        {
                            tile.TileType = (ushort)ModContent.TileType<Manastone>();
                            //tile.WallType = (ushort)ModContent.WallType<ManastoneWall>();
                            continue;
                        }

                        tile.TileType = (ushort)ModContent.TileType<OvergrowthDirt>();
                    }
                }
            });

            EnchantedOvergrowthGen.OvergrowthPos = new Point(overgrowthPosX, overgrowthPosY);
        }

        public bool TilesToAvoidNearby(int size, int x, int y)
        {
            int halfSize = (int)(size * 0.5f);
            int[] iceTiles = 
            [
                TileID.IceBlock, TileID.SnowBlock, TileID.BlueDungeonBrick, 
                TileID.GreenDungeonBrick, TileID.PinkDungeonBrick, TileID.Sandstone, 
                TileID.Sand, TileID.CrimsonGrass, TileID.CorruptGrass,
                TileID.JungleGrass, TileID.Crimstone, TileID.Ebonstone
            ];

            for (int i = x - halfSize; i < x + halfSize; i++)
            {
                for (int j = y; j < y + halfSize; j++)
                {
                    if (Framing.GetTileSafely(i, j).HasTile && iceTiles.Contains(Framing.GetTileSafely(i, j).TileType))
                        return true;
                }
            }

            return false;
        }
    }

    public class OvergrowthFoliagePass : GenPass
    {
        public OvergrowthFoliagePass(string name, float loadWeight) : base(name, loadWeight)
        {
        }

        protected override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Growing toxic enchanted plants";

            int size = (int)(Main.maxTilesX * 0.065f);
            int overgrowthStartY = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f));

            FastParallel.For(20, Main.maxTilesX - 20, (from, to, _) =>
            {
                for (int i = from; i <= to; i++)
                {
                    for (int j = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f)) - 20; j <= Main.maxTilesY - 20; j++)
                    {
                        Tile tile = Framing.GetTileSafely(i, j);

                        if (tile.TileType != ModContent.TileType<OvergrowthDirt>())
                            continue;

                        AdjacencyData<bool> tileData = GetAdjacentTiles(i, j, (tile) => tile.HasTile);

                        if (tile.TileType == ModContent.TileType<OvergrowthDirt>() && !tileData.top)
                            tile.TileType = (ushort)ModContent.TileType<OvergrowthGrass>();
                    }
                }
            });

            int innerBoundX = EnchantedOvergrowthGen.OvergrowthPos.X - (int)(size * 0.5f);
            int outerBoundX = EnchantedOvergrowthGen.OvergrowthPos.X + (int)(size * 0.5f);


            int[] bigTypes =
            [
                TwilightEgress.Instance.Find<ModTile>("OvergrowthTreeBaseLarge").Type,
                TwilightEgress.Instance.Find<ModTile>("OvergrowthTreeBaseLarge2").Type
            ];

            GenerateTrees(size, bigTypes, 0);
            GenerateTrees(size, [ TwilightEgress.Instance.Find<ModTile>("OvergrowthTreeBaseMedium").Type ], 1);
            GenerateTrees(size, [ TwilightEgress.Instance.Find<ModTile>("OvergrowthTreeBaseSmall").Type ], 2);
        }

        public void GenerateTrees(int size, int[] trees, int displacement)
        {
            int innerBoundX = EnchantedOvergrowthGen.OvergrowthPos.X - (int)(size * 0.5f);
            int outerBoundX = EnchantedOvergrowthGen.OvergrowthPos.X + (int)(size * 0.5f);

            for (int i = innerBoundX; i < outerBoundX; i++)
            {
                int j = (int)(Main.worldSurface - (Main.maxTilesY * 0.125f));
                bool success = false;
                while (j <= Main.maxTilesY - 20 && !success)
                {
                    Tile tile = Framing.GetTileSafely(i, j);

                    if (!tile.HasTile)
                    {
                        j++;
                        continue;
                    }

                    if (tile.TileType != (ushort)ModContent.TileType<OvergrowthGrass>())
                    {
                        success = true;
                        break;
                    }

                    PlaceTree(i, j - 1, WorldGen.genRand.Next(9, 15), trees, displacement);
                    success = true;
                }
            }
        }

        public static bool PlaceTree(int i, int j, int height, int[] typesToPlace, int displacement)
        {
            if (Framing.GetTileSafely(i, j).HasTile)
                return false;

            // check if any tiles are in the way of the trunk
            List<Point> pointsToPlaceTree = new List<Point>();

            for (int y = -1; y > -height; y--)
            {
                pointsToPlaceTree.Add(new Point(i, j + y - 3));

                if (Framing.GetTileSafely(i, j + y - 3).HasTile)
                    return false;
            }

            // attempt to place the base
            int tileType = WorldGen.genRand.Next(typesToPlace);
            int placeStyle = tileType == TwilightEgress.Instance.Find<ModTile>("OvergrowthTreeBaseLarge2").Type ? WorldGen.genRand.Next(2) : WorldGen.genRand.Next(4);

            WorldGen.PlaceTile(i, j, tileType, mute: true, style: placeStyle);
            if (Framing.GetTileSafely(i, j).TileType != tileType)
                return false;

            // place the trunk
            foreach (Point point in pointsToPlaceTree)
            {
                WorldGen.PlaceTile(point.X, point.Y + displacement, ModContent.TileType<OvergrowthTree>(), mute: true);
            }

            return true;
        }
    }
}
