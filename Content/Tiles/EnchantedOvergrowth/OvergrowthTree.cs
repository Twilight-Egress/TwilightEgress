using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Linq;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using TwilightEgress.Core;
using static TwilightEgress.TwilightEgressUtilities;

namespace TwilightEgress.Content.Tiles.EnchantedOvergrowth
{
    public class OvergrowthTree : ModTile
    {
        private Asset<Texture2D> topsTexture;
        private Asset<Texture2D> topsGlowTexture;

        public override string Texture => base.Texture.Replace("Content", "Assets/Textures");

        public override void SetStaticDefaults()
        {
            TileID.Sets.IsATreeTrunk[Type] = true;
            Main.tileAxe[Type] = true;
            RegisterItemDrop(ItemID.Wood);
            AddMapEntry(new Color(80, 55, 74));

            topsTexture = ModContent.Request<Texture2D>("TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/OvergrowthTree_Tops");
            topsGlowTexture = ModContent.Request<Texture2D>("TwilightEgress/Assets/Textures/Tiles/EnchantedOvergrowth/OvergrowthTree_TopsGlow");
        }

        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int frameX = 22 * (j % 4);
            int frameY = 22 * (i % 2);
            Color paintColor = WorldGen.paintColor(Framing.GetTileSafely(i, j).TileColor);
            Rectangle sourceRectangle = new Rectangle(frameX, frameY, 20, 20);
            spriteBatch.DrawTileTexture(TextureAssets.Tile[Type].Value, i, j, sourceRectangle, paintColor, 0f, Vector2.Zero + new Vector2(2, 2));

            if (Framing.GetTileSafely(i, j - 1).TileType == Type)
                return false;

            frameX = 36 * (i % 4);
            sourceRectangle = new Rectangle(frameX, 0, 36, 70);
            spriteBatch.DrawTileTexture(topsTexture.Value, i, j, sourceRectangle, paintColor, 0.0f, new(10, 70));
            spriteBatch.DrawTileTexture(topsGlowTexture.Value, i, j, sourceRectangle, paintColor, 0.0f, new(10, 70), lighted: false);

            return false;
        }

        public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (fail || effectOnly)
                return;

            Framing.GetTileSafely(i, j).HasTile = false;

            AdjacencyData<int> adjacencyData = GetAdjacentTiles(i, j, (tile) => (int)tile.TileType);
            int[] treeBases = 
            [
                Mod.Find<ModTile>("OvergrowthTreeBaseLarge").Type,
                Mod.Find<ModTile>("OvergrowthTreeBaseLarge2").Type,
                Mod.Find<ModTile>("OvergrowthTreeBaseMedium").Type,
                Mod.Find<ModTile>("OvergrowthTreeBaseSmall").Type
            ];

            if (adjacencyData.top == Type)
                WorldGen.KillTile(i, j - 1);
            if (adjacencyData.bottom == Type || treeBases.Contains(adjacencyData.bottom))
                WorldGen.KillTile(i, j + 1);
        }
    }
}
